using NLog;
using Player.Core.Net;
using Player.Model.Action;
using Player.Util;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace Player.Core.Action
{
    /// <summary>
    /// Sends a message over TCP.<para />
    /// Uses '\n' as delimiter at the end of the message. Message is encoded in ASCII.
    /// </summary>
    class TcpAction : BaseAction<TcpActionParameter>
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private static readonly char DELIMITER = '\n';


        private readonly TcpConnectionPool connections;

        private byte[] data;


        public TcpAction(ActionParameter param, TcpConnectionPool conPool)
            :base(param)
        {
            connections = conPool;
            data = Encoding.ASCII.GetBytes(Param.Message + DELIMITER);
        }


        public override void DoAction()
        {
            try
            {
                connections.SendMessage(Param.Destination, data);
            }
            catch (Exception e)
            {
                logger.Error(ExceptionUtil.Format(e));
            }
        }
    }
}
