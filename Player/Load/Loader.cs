using Player.Model;
using System;

namespace Player.Load
{
    /// <summary>
    /// Is responsible for generating keyboard model objects from keyboard files.
    /// </summary>
    interface Loader
    {
        /// <returns>A valid <see cref="KeyboardModel"/> instance.</returns>
        /// <exception cref="LoaderException">If any error occurs.</exception>
        KeyboardModel LoadKeyboard(string path);
    }
}
