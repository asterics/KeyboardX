using Player.Model;
using System;

namespace Player.Core.Status
{
    /// <summary>
    /// Which buttons/areas have changed their status.
    /// </summary>
    /// <remarks>
    /// Should only IDs be sent, or in addition the new status too?
    /// => Because all listeners should also have a reference to grid status, they are able to get button status anyway.
    ///    Therefore we don't send status in the event for now. Otherwise we would have a problem with current structure.
    /// </remarks>
    public class StatusChangeEventArgs : EventArgs
    {
        public static readonly new StatusChangeEventArgs Empty = new StatusChangeEventArgs(ButtonGroup.Empty);

        public ButtonGroup ChangedButtons { get; protected set; }


        public StatusChangeEventArgs(ButtonGroup changes)
        {
            ChangedButtons = changes;
        }
    }
}
