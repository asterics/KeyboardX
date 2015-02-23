using Player.Model;
using System;

namespace Player.Core.Gui
{
    /// <summary>
    /// When "something" on a grid got clicked. <paramref name="ButtonId"/> can be null (or empty)!
    /// </summary>
    /// <remarks>
    /// Why does it make sense to click "something", e.g. raise an empty click event?
    /// => If desired behavior is, that clicking somewhere on grid should yield a trigger event for scanner, an empty click event is the way to go.
    /// </remarks>
    public interface ClickEventArgs
    {
        string ButtonId { get; }
    }
}
