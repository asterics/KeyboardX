using System;

namespace Player.Model.Action
{
    class TcpActionParameter : BaseActionParameter
    {
        public static readonly string ActionTypeKeyword = "tcp";


        public TcpDestinationModel Destination { get; private set; }

        public string Message { get; private set; }


        public TcpActionParameter(TcpDestinationModel dst, string message)
        {
            Destination = dst;
            Message = message;
        }


        public override string ToString()
        {
            TcpDestinationModel dst = Destination;
            if (dst.Id == null)
                return String.Format("{0} ( message = \"{1}\", host = {2}, port = {3} )", GetType().Name, Message, dst.Host, dst.Port);
            else
                return String.Format("{0} ( message = \"{1}\", destId = '{2}' )", GetType().Name, Message, dst.Id);
        }
    }
}
