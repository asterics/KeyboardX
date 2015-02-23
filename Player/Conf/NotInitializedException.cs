using System;

namespace Player.Conf
{
    [Serializable]
    class NotInitializedException : Exception
    {
        public NotInitializedException(string msg)
            : base(msg)
        {
            // NOP
        }
    }
}
