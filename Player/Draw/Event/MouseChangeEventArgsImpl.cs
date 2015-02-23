using Player.Core.Gui;
using Player.Draw.Element;
using Player.Model;
using System;

namespace Player.Draw.Event
{
    class MouseChangeEventArgsImpl : EventArgs, MouseChangeEventArgs
    {
        public ButtonGroup MouseOver { get; private set; }

        public ButtonGroup MouseOut { get; private set; }


        /// <remarks>
        /// As only one mouse pointer is supported by <see cref="System.Windows.Forms"/>, we only provide constructor for one button.
        /// </remarks>
        public MouseChangeEventArgsImpl(DrawableButton mouseOver, DrawableButton mouseOut)
        {
            if (mouseOver != null)
                MouseOver = new ButtonGroup(mouseOver.Id);
            else
                MouseOver = ButtonGroup.Empty;

            if (mouseOut != null)
                MouseOut = new ButtonGroup(mouseOut.Id);
            else
                MouseOut = ButtonGroup.Empty;
        }
    }
}
