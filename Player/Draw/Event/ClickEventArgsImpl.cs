using Player.Core.Gui;
using System;

namespace Player.Draw.Event
{
    class ClickEventArgsImpl : EventArgs, ClickEventArgs
    {
        // See ClickEventArgs why it makes sense to raise an empty click event.
        public static readonly new ClickEventArgsImpl Empty = new ClickEventArgsImpl();

        public string ButtonId { get; private set; }


        private ClickEventArgsImpl()
        {
            ButtonId = null;
        }

        public ClickEventArgsImpl(string buttonId)
        {
            ButtonId = buttonId;
        }
    }
}
