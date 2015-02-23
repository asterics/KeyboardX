using Player.Core.Gui;
using System;

namespace Player.Draw.Event
{
    class RedrawEventArgsImpl : EventArgs, RedrawEventArgs
    {
        public static readonly new RedrawEventArgsImpl Empty = new RedrawEventArgsImpl();


        private RedrawEventArgsImpl()
        {
            // NOP
        }
    }
}
