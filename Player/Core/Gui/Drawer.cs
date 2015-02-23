using Player.Core.Status;
using System;
using System.Drawing;

namespace Player.Core.Gui
{
    /// <summary>
    /// Is responsible for drawing a grid into a window form and further for interaction that happens with this form (by the user).
    /// <para />
    /// <see cref="Draw()"/> is called from GUI thread.
    /// </summary>
    /// TODOs:
    ///  TODO 5: if button contains icon and text, text should be closer at icon (https://github.com/s3huber/KeyboardX/issues/5)
    public interface Drawer : IDisposable
    {
        event EventHandler<ClickEventArgs> Click;

        event EventHandler<MouseChangeEventArgs> MouseChange;

        event EventHandler<RedrawEventArgs> Redraw;

        void Draw(Graphics g, Size size);

        /// <summary>
        /// Used by <see cref="PlayerForm"/> to signal <see cref="Drawer"/> that a click happened. <see cref="Drawer"/> is responsible to decide 
        /// what was clicked (if at all). It's only a click, if mouse down AND mouse up happens on the same element. It's only a click, if 
        /// <paramref name="mouseDown"/> AND <paramref name="mouseUp"/> happens on the same element.<para />
        /// <see cref="Click"/> event is used to signal an actual click.
        /// </summary>
        void OnClick(Point mouseDown, Point mouseUp);

        void OnMouseMove(Point location);

        void OnStatusChange(Object sender, StatusChangeEventArgs e);

        /// <summary>
        /// Used for reseting the state of drawer (on grid switch) and resize elements based on current form size.
        /// </summary>
        /// <param name="size">Current size of the form onto which is drawn.</param>
        void Reset(Size size);
    }
}
