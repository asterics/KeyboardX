using System;

namespace Player.Model.Action
{
    class TTSActionParameter : BaseActionParameter
    {
        public static readonly string ActionTypeKeyword = "tts";


        public string Message { get; private set; }

        public TTSActionParameter(string message)
        {
            Message = message;
        }
    }
}
