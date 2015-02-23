using Player.Model;
using System;

namespace Player.Load.Element
{
    class TcpDestination : TcpDestinationModel
    {
        public string Id { get; private set; }

        public string Host { get; private set; }

        public int Port { get; private set; }


        public TcpDestination(string id)
        {
            Id = id;
        }

        public TcpDestination(string host, int port)
        {
            Host = host;
            Port = port;
        }
    }
}
