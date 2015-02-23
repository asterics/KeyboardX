using System;

namespace Player.Core.Element
{
    /// <summary>
    /// Used to abstract building of <see cref="Keyboard"/> object.
    /// </summary>
    interface KeyboardBuilder
    {
        Keyboard BuildKeyboard();
    }
}
