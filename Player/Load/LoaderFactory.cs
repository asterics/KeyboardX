using System;

namespace Player.Load
{
    /// <remarks>
    /// This is the only dependency to implementation that should exist to this namespace from outside.
    /// </remarks>
    class LoaderFactory
    {
        public static Loader CreateLoader()
        {
            return new KeyboardLoader();
        }
    }
}
