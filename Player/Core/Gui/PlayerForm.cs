using NLog;
using Player.Core;
using Player.Draw;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace Player.Core.Gui
{
    /// <summary>
    /// This form is used for drawing. At the moment there is no menu or stuff.<para />
    /// Logically PlayerForm is a class of the Draw namespace.
    /// </summary>
    public partial class PlayerForm : Form
    {
        /// <summary>When resizing fast, CPU gets busy from drawing up to hundreds frames per second, which is not necessary.</summary>
        public static readonly int MIN_RESIZE_REDRAW_TIME = 12;

        private static Logger logger = LogManager.GetCurrentClassLogger();


        private readonly string initialText;

        private Drawer _drawer;
        protected Drawer Drawer
        {
            get
            {
                return _drawer;
            }

            set
            {
                if (_drawer != null)
                    _drawer.Redraw -= OnRedraw;

                _drawer = value;
                _drawer.Redraw += OnRedraw;
            }
        }

        protected Point mouseDown;

        protected bool resizing;
        

        public PlayerForm()
        {
            InitializeComponent();
            ResizeRedraw = true;
            initialText = Text;
        }


        /// <summary>
        /// Switches drawer, corresponding events and text of form.
        /// </summary>
        public void SwitchDrawer(Drawer drawer, string keyboardName, string gridName)
        {
            MethodInvoker switchDrawerAndText = delegate()
            {
                Drawer = drawer;
                Drawer.Reset(ClientSize);
                AnnounceMousePosition();
                Text = String.Format("{0} - {1}/{2}", initialText, keyboardName, gridName);
                Invalidate();
            };

            if (InvokeRequired)
                Invoke(switchDrawerAndText);
            else
                switchDrawerAndText();
        }

        private void AnnounceMousePosition()
        {
            Point mousePos = PointToClient(MousePosition);
            if (ClientRectangle.Contains(mousePos))
                Drawer.OnMouseMove(mousePos);
        }

        protected void OnRedraw(Object sender, RedrawEventArgs e)
        {
            logger.Trace("OnRedraw() received...");

            // TODO 3: [performance] invalidate only what really changed (could be tricky)
            Invalidate();
        }

        /// <remarks>
        /// What is the difference between ClipRectangle and Graphics.ClipBounds?
        /// => http://stackoverflow.com/a/12062755/3083621
        /// 
        /// Let's use Graphics.VisibleClipBounds for determining what to draw for now. Graphics.Clip seems to be set by the system.
        /// </remarks>
        protected override void OnPaint(PaintEventArgs e)
        {
            int startTick = Environment.TickCount;

            base.OnPaint(e);  // first call the base in this case

            if (Drawer != null)
                Drawer.Draw(e.Graphics, ClientSize);

            if (resizing)
            {
                int sleep = MIN_RESIZE_REDRAW_TIME - (Environment.TickCount - startTick);
                if (sleep > 0 && sleep <= MIN_RESIZE_REDRAW_TIME)
                    Thread.Sleep(sleep);
            }
        }

        protected override void OnResizeBegin(EventArgs e)
        {
            resizing = true;
            base.OnResizeBegin(e);
        }

        protected override void OnResizeEnd(EventArgs e)
        {
            resizing = false;
            base.OnResizeEnd(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (Drawer != null)
                Drawer.OnMouseMove(e.Location);
            base.OnMouseMove(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            // drawer needs to know when mouse leaves window, cause it's responsible for mouse change events
            if (Drawer != null)
                Drawer.OnMouseMove(new Point(-1, -1));
            base.OnMouseLeave(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            mouseDown = e.Location;
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            Point mouseUp = e.Location;
            if (Drawer != null)
                Drawer.OnClick(mouseDown, mouseUp);
            base.OnMouseUp(e);
        }
    }
}
