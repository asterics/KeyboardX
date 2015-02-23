using NLog;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Player.Core.Net
{
    /// <summary>
    /// Used for development or debugging.<para />
    /// Listens on a given port and logs incoming messages. Handles only one client!
    /// </summary>
    class TcpSink
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();


        private readonly int port;

        private readonly int timeout;

        private byte[] data;

        private TcpListener listener;


        public TcpSink(int port, int timeout = -1)
        {
            this.port = port;
            this.timeout = timeout;
            data = new byte[128];
        }


        public void Activate()
        {
            if (listener != null)
                throw new InvalidOperationException("It seems instance is already activated!");

            try
            {
                listener = TcpListener.Create(port);
                listener.Start();
                logger.Info("Listening on port {0}.", port);

                Func<Task> listen = ListenAsync;
                Task.Factory.StartNew(listen, TaskCreationOptions.LongRunning);
            }
            catch (Exception e)
            {
                logger.Error("Exception while activating!\n{1}", e);
            }

            
        }

        public void Deactivate()
        {
            if (listener == null)
                throw new InvalidOperationException("It seems instance was not activated correctly!");

            try { listener.Stop(); }
            catch (Exception e)
            {
                logger.Error(e);
            }
            finally { listener = null; }
        }

        private async Task ListenAsync()
        {
            while (true)
            {
                if (listener == null)
                    break;

                TcpClient client = null;
                try
                {
                    client = await listener.AcceptTcpClientAsync();
                    logger.Info("Connection received from {0}.", client.Client.RemoteEndPoint);

                    if (timeout > 0)
                        client.ReceiveTimeout = timeout;

                    using (NetworkStream stream = client.GetStream())
                        ReadFrom(stream);
                    
                }
                catch (Exception e)
                {
                    if (e is ObjectDisposedException || e is InvalidOperationException)
                    {
                        logger.Debug("Deactivated while waiting for a connection -> stop listening!");
                        break;
                    }
                    else if (e is IOException && e.InnerException != null && e.InnerException is SocketException)
                    {
                        SocketException se = (SocketException)e.InnerException;
                        logger.Debug("Connection ended because: {0}", se.SocketErrorCode);
                    }
                    else
                        logger.Error(e);
                }
                finally
                {
                    if (client != null)
                        client.Close();
                }
            }

            logger.Info("Listening ended.");
        }

        private void ReadFrom(NetworkStream stream)
        {
            while (true)
            {
                string input = null;
                int count = stream.Read(data, 0, data.Length);

                if (count > 0)
                    input = Encoding.ASCII.GetString(data, 0, count);

                if (input == null)
                {
                    logger.Debug("Client disconnected.");
                    break;
                }
                else if (input.StartsWith("!quit!"))
                {
                    logger.Debug("Magic quit message received.");
                    break;
                }
                else
                {
                    input = Regex.Replace(input, @"\r\n?|\n", "<br>");  // replace linefeed or newline by a symbolic '<br>'
                    logger.Info("Received: '{0}'", input);
                }
            }
        }
    }
}
