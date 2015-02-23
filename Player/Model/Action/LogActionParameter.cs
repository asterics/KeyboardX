using System;

namespace Player.Model.Action
{
    class LogActionParameter : BaseActionParameter
    {
        public static readonly string ActionTypeKeyword = "log";


        public string Message { get; private set; }

        public LogActionParameter(string message)
        {
            Message = message;
        }
    }
}
