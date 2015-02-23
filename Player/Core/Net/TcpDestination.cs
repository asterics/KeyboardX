using NLog;
using Player.Model;
using System;
using System.Net;

namespace Player.Core.Net
{
    /// <summary>
    /// Combines a host and port like <see cref="System.Net.IPEndPoint"/>, but also adds an id for referencing.
    /// </summary>
    class TcpDestination : Model.TcpDestinationModel
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public string Id { get; private set; }

        public string Host { get; private set; }

        public int Port { get; private set; }


        public TcpDestination(TcpDestinationModel model)
        {
            if (model.Id != null)
                Init(model.Id, model.Host, model.Port);
            else
            {
                string id = GenerateId(model.Host, model.Port);
                Init(id, model.Host, model.Port);
            }
        }

        public TcpDestination(string id, string host, int port)
        {
            Init(id, host, port);
        }


        private void Init(string id, string host, int port)
        {
            Id = CheckedId(id);
            Host = CheckedHost(host);
            Port = CheckedPort(port);
        }

        private string CheckedId(string id)
        {
            if (String.IsNullOrEmpty(id))
                throw new ArgumentException("Argument 'id' may not be null or empty!");
            else
                return id;
        }

        private string CheckedHost(string host)
        {
            if (Uri.CheckHostName(host) == UriHostNameType.Unknown)
                throw new ArgumentException("Argument 'host' is not valid!");
            else
                return host;
        }

        private int CheckedPort(int port)
        {
            if (port < IPEndPoint.MinPort || port > IPEndPoint.MaxPort)
                throw new ArgumentOutOfRangeException("Argument 'port' is out of valid range!");
            else
                return port;
        }

        private string GenerateId(string host, int port)
        {
            return String.Format("{0}:{1}", host, port);
        }

        public bool Equals(TcpDestination other)
        {
            if (Id.Equals(other.Id) && 
                    Host.Equals(other.Host, StringComparison.OrdinalIgnoreCase) && 
                    Port == other.Port)
                return true;
            else
                return false;
        }
    }
}
