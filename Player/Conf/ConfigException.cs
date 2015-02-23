using System;

namespace Player.Conf
{
    [Serializable]
    class ConfigException : Exception
    {
        public ConfigException(string message) 
            : base(message)
        {
            // NOP
        }

        public ConfigException(string message, Exception innerException)
            : base(message, innerException)
        {
            // NOP
        }
    }
}
