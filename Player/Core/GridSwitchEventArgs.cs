using System;

namespace Player.Core
{
    /// <summary>
    /// A grid switch event should be considered to come in either by action or over network at any time.
    /// </summary>
    class GridSwitchEventArgs : EventArgs
    {
        public string GridId { get; private set; }


        public GridSwitchEventArgs(string gridId)
        {
            GridId = gridId;
        }
    }
}
