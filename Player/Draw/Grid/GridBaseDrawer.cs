using NLog;
using Player.Core.Gui;
using Player.Core.Status;
using Player.Draw.Button;
using Player.Draw.Element;
using Player.Draw.Event;
using Player.Model;
using System;
using System.Diagnostics;
using System.Drawing;

namespace Player.Draw.Grid
{
    /// <summary>
    /// The base of all grid drawers.
    /// </summary>
    /// <remarks>
    /// How should we deal with (WIDTH % COLS != 0)?
    /// => Share spare pixels first right then left on the outside of grid.
    /// </remarks>
    /// TODOs:
    ///  TODO 6: [performance] try to enhance scaling of images (use brush/texture)
    ///  TODO 5: [performance] add cache for scaled image to drawable button
    ///  TODO 4: [performance] add a border selector that uses 2 layers, 1st for buttons, 2nd for border drawing
    abstract class GridBaseDrawer : Drawer
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();


        public event EventHandler<ClickEventArgs> Click;

        public event EventHandler<MouseChangeEventArgs> MouseChange;

        public event EventHandler<RedrawEventArgs> Redraw;

        protected DrawableGrid grid;

        protected ButtonBaseDrawer drawer;

        protected Size CanvasSize { get; private set; }

        /// <summary>Keep track of on which button mouse was last time.</summary>
        private DrawableButton mouseOverButton;

        private Stopwatch sw = new Stopwatch();  // for dev only


        protected GridBaseDrawer(GridModel model, GridStatus status, ButtonBaseDrawer drawer)
        {
            grid = new DrawableGrid(model, status);
            this.drawer = drawer;
        }


        public virtual void Dispose()
        {
            drawer.Dispose();
            grid.Dispose();
        }

        public virtual void Reset(Size size)
        {
            mouseOverButton = null;
            CanvasSize = size;
            ResizeGrid();
        }

        public void Draw(Graphics g, Size size)
        {
            sw.Restart();

            if (!size.Equals(CanvasSize))
            {
                CanvasSize = size;
                ResizeGrid();
            }

            DrawBack(g);
            DrawGrid(g);

            sw.Stop();
            logger.Debug("draw needed {0}ms", sw.ElapsedMilliseconds);

            // TODO 1: [performance] eventually play around with this values
            //g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Low;
            //g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            //g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.None;
            //g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
            //g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixel;
        }

        protected abstract void ResizeGrid();

        protected abstract void DrawGrid(Graphics g);

        protected virtual void DrawBack(Graphics g)
        {
            g.FillRectangle(grid.BackBrush, 0, 0, CanvasSize.Width, CanvasSize.Height);
            // background image could be possible here too
        }

        // See comment in interface.
        public void OnClick(Point mouseDown, Point mouseUp)
        {
            foreach (var btn in grid)
            {
                Rectangle btnPos = btn.PixelPosition;
                if (btnPos.Contains(mouseDown) && btnPos.Contains(mouseUp))
                {
                    RaiseClickEvent(new ClickEventArgsImpl(btn.Id));
                    return;
                }
            }

            // let's even raise a click event if no button directly got clicked (@see [ClickEventArgs])
            RaiseClickEvent(ClickEventArgsImpl.Empty);
        }

        protected void RaiseClickEvent(ClickEventArgs e)
        {
            logger.Trace("Raising click event for button '{0}'...", e.ButtonId ?? "{NONE}");

            EventHandler<ClickEventArgs> handler = Click;
            if (handler != null)
                handler(this, e);
        }

        public void OnMouseMove(Point location)
        {
            DrawableButton mouseOutButton = null;

            if (mouseOverButton != null)
            {
                if (mouseOverButton.PixelPosition.Contains(location))
                    return;  // nothing new happened
                else
                {
                    mouseOutButton = mouseOverButton;
                    mouseOverButton = null;
                }
            }

            foreach (var btn in grid)
            {
                if (btn.PixelPosition.Contains(location))
                    mouseOverButton = btn;
            }

            if (mouseOverButton != null || mouseOutButton != null)
            {
                MouseChangeEventArgs args = new MouseChangeEventArgsImpl(mouseOverButton, mouseOutButton);
                RaiseMouseChangeEvent(args);
            }
        }

        protected void RaiseMouseChangeEvent(MouseChangeEventArgs e)
        {
            logger.Trace("Raising mouse change event...");

            EventHandler<MouseChangeEventArgs> handler = MouseChange;
            if (handler != null)
                handler(this, e);
        }

        public void OnStatusChange(Object sender, StatusChangeEventArgs e)
        {
            logger.Trace("OnStatusChange() received...");

            RaiseRedrawEvent(RedrawEventArgsImpl.Empty);
        }

        protected void RaiseRedrawEvent(RedrawEventArgs e)
        {
            logger.Trace("Raising redraw event...");

            EventHandler<RedrawEventArgs> handler = Redraw;
            if (handler != null)
                handler(this, e);
        }
    }
}
