using NLog;
using Player.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Linq;

namespace Player.Core.Net
{
    /// <summary>
    /// Manages TCP connections and provides services therefor, like sending message.
    /// Connections are shared in a sophisticated way.
    /// </summary>
    /// <remarks>
    /// Could also be called 'TcpService' or split up into both 'TcpConnectionPool' and 'TcpService' if it grows further.
    /// 
    /// Error detection seems still not be perfect, although a lot of effort was put into it.
    /// </remarks>
    class TcpConnectionPool : IDisposable
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private static readonly int TIMEOUT = 30;  // TODO 4: [conf] consider to get timeout from config

        /// <summary>How often to retry to send message. One retry means two tries in total.</summary>
        private static readonly int RETRIES = 1;


        private readonly object syncLock = new object();

        private Dictionary<string, Connection> connections;
        

        public TcpConnectionPool()
        {
            connections = new Dictionary<string, Connection>();
        }


        /// <summary>
        /// New destinations override existing ones, if they have the same id.
        /// </summary>
        public void Add(TcpDestinationModel model)
        {
            TcpDestination dst = new TcpDestination(model);

            lock (syncLock)
            {
                if (connections.ContainsKey(dst.Id))
                {
                    TcpDestination old = connections[dst.Id].Destination;
                    if (dst.Equals(old))
                        logger.Warn("Ignored add operation, because identical {0} is already there.", typeof(TcpDestination));
                    else
                        ReplaceConnection(dst);
                }
                else
                    AddEntry(dst);
            }
        }

        /// <remarks>
        /// When using the stream directly, one should access it asynchronously, cause no locking happens.
        /// For multi-threaded access MSDN suggests the following:
        /// "If you want to process your I/O using separate threads, consider using the BeginWrite and EndWrite methods..."
        /// [http://msdn.microsoft.com/en-us/library/System.Net.Sockets.NetworkStream%28v=vs.110%29.aspx]
        /// </remarks>
        public NetworkStream GetStream(TcpDestinationModel dst)
        {
            Connection con = GetConnection(dst);
            CheckConnection(con);
            return con.Client.GetStream();
        }

        public void SendMessage(TcpDestinationModel dst, byte[] msg)
        {
            Connection con = GetConnection(dst);
            int tries = RETRIES + 1;

            lock (con)
            {
                while (tries-- > 0)
                {
                    CheckConnection(con);

                    bool sent;
                    try
                    {
                        TcpClient client = con.Client;
                        client.GetStream().Write(msg, 0, msg.Length);
                        sent = SeemsHaveWorked(client);
                    }
                    catch (IOException)
                    {
                        sent = false;
                    }

                    if (sent)
                        return;
                }
            }

            throw new Exception("Message couldn't be sent!");
        }

        public void Dispose()
        {
            lock (syncLock)
            {
                foreach (var con in connections.Values)
                {
                    if (con.Client != null)
                        con.Client.Close();
                }

                connections.Clear();
            }
        }

        private Connection AddEntry(TcpDestination dst)
        {
            Connection con = new Connection();
            con.Destination = dst;

            var active = LookUpActive(dst.Host, dst.Port);
            if (active != null)
                con.Client = active.Client;

            connections.Add(dst.Id, con);
            return con;
        }

        private Connection GetConnection(TcpDestinationModel dst)
        {
            Connection con;

            lock (syncLock)
            {
                if (dst.Id != null)
                {
                    if (connections.ContainsKey(dst.Id))
                        con = connections[dst.Id];
                    else
                        throw new ArgumentException(String.Format("There is no connection with id '{0}' available!", dst.Id));
                }
                else
                {
                    TcpDestination dest = new TcpDestination(dst);

                    if (connections.ContainsKey(dest.Id))
                        con = connections[dest.Id];
                    else
                        con = AddEntry(dest);
                }
            }

            return con;
        }

        private Connection LookUpActive(string host, int port)
        {
            var activeCons = connections.Select(x => x.Value).Where(x => x.Client != null);

            foreach (var con in activeCons)
            {
                TcpDestination dst = con.Destination;
                if (dst.Host.Equals(host) && dst.Port == port)
                    return con;
            }

            return null;
        }

        private void CheckConnection(Connection con)
        {
            TcpClient client = con.Client;
            TcpDestination dst = con.Destination;

            try
            {
                lock (con)
                {
                    if (client == null)  // i.e. initial connection
                    {
                        var active = LookUpActive(dst.Host, dst.Port);
                        if (active != null)
                            con.Client = active.Client;
                        else
                        {
                            logger.Debug("Connecting to {0}...", dst.Id);
                            con.Client = CreateConnectedClient(dst);
                        }
                    }
                    else
                    {
                        if (!client.Connected)
                        {
                            logger.Debug("Reconnecting to {0}...", dst.Id);
                            client.Close();
                            con.Client = CreateConnectedClient(dst);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                string msg = String.Format("Exception occurred while connecting to TCP destination '{0}'!", dst.Id);
                throw new Exception(msg, e);
            }
        }

        private TcpClient CreateConnectedClient(TcpDestination dst)
        {
            TcpClient client = new TcpClient();
            client.NoDelay = true;
            client.SendTimeout = TIMEOUT;

            client.Connect(dst.Host, dst.Port);

            return client;
        }      

        private void ReplaceConnection(TcpDestination newDst)
        {
            Connection con = connections[newDst.Id];

            if (con.Client != null)
                con.Client = null;  // don't close connection, cause maybe used by other destination

            con.Destination = newDst;
        }

        /// <summary>
        /// Unfortunately problems with connection can only be detected after a failed read/write. This method checks if connection after a 
        /// read/write seems still fine. Therefor a 'pseudo write' is used. If it fails, something's wrong.
        /// </summary>
        /// <returns><c>false</c> if connection seems broken, <c>true</c> otherwise.</returns>
        private bool SeemsHaveWorked(TcpClient client)
        {
            if (client.Connected)
            {
                // pseudo write, helps detecting connection problems
                try { client.GetStream().Write(new byte[1], 0, 0); }
                catch (IOException) { return false; }
            }
            else
                return false;

            return true;
        }


        protected class Connection
        {
            public TcpDestination Destination { get; set; }

            public TcpClient Client { get; set; }
        }
    }
}
