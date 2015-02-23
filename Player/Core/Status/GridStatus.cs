using System;

namespace Player.Core.Status
{
    /// <summary>
    /// Combines button status for all buttons of a grid.
    /// </summary>
    public interface GridStatus
    {
        //ButtonStatus this[int x, int y] { get; }  // seems not to be needed

        ButtonStatus this[string buttonId] { get; }
    }
}
