using Player.Model;
using System;

namespace Player.Core.Gui
{
    /// <summary>
    /// Signals mouse over changes for buttons/areas. Which buttons mouse is currently over and which buttons mouse just left.
    /// </summary>
    public interface MouseChangeEventArgs
    {
        ButtonGroup MouseOver { get; }

        ButtonGroup MouseOut { get; }
    }
}
