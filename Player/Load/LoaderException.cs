using System;

namespace Player.Load
{
    [Serializable]
    class LoaderException : Exception
    {
        public LoaderException()
            : base()
        {
            // NOP
        }

        public LoaderException(string message) 
            : base(message)
        {
            // NOP
        }

        public LoaderException(string message, Exception innerException)
            : base(message, innerException)
        {
            // NOP
        }
    }
}
