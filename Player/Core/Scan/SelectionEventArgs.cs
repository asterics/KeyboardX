using Player.Model;
using System;

namespace Player.Core.Scan
{
    /// <summary>
    /// Contains selection changes for buttons/areas.
    /// Or in other words, which buttons/areas where selected before and which are selected now.
    /// </summary>
    /// TODOs:
    ///  TODO 3: think about special cases, what if a button has dimension > 1 and is inside selected and unselected
    class SelectionEventArgs : EventArgs
    {
        public static readonly new SelectionEventArgs Empty = new SelectionEventArgs(ButtonGroup.Empty, ButtonGroup.Empty);


        public ButtonGroup Selected { get; protected set; }

        public ButtonGroup Unselected { get; protected set; }


        public SelectionEventArgs(ButtonGroup selected, ButtonGroup unselected)
        {
            Selected = selected;
            Unselected = unselected;
        }
    }
}
