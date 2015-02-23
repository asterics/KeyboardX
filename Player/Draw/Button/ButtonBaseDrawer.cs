using NLog;
using Player.Core.Status;
using Player.Draw.Element;
using System;
using System.Drawing;

namespace Player.Draw.Button
{
    /// <summary>
    /// The base of all button drawers.
    /// </summary>
    /// <remarks>
    /// How about <c>Rectangle</c> vs. <c>RectangleF</c>?
    /// => It's better to use <c>Rectangle</c> at first, cause half a pixel can't be drawn anyway.
    /// </remarks>
    abstract class ButtonBaseDrawer : IDisposable
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>This instance can decorate another drawer.</summary>
        private ButtonBaseDrawer drawer;


        public ButtonBaseDrawer()
        {
            // NOP
        }

        public ButtonBaseDrawer(ButtonBaseDrawer drawer)
        {
            this.drawer = drawer;
        }

        public void DrawButton(Graphics g, DrawableButton btn)
        {
            DrawButton(g, btn, btn.Status);
        }

        public void DrawButton(Graphics g, DrawableButton btn, ButtonStatus status)
        {
            // first let decorated drawer draw
            if (drawer != null)
                drawer.DrawButton(g, btn, status);

            try { Draw(g, btn, status); }
            catch (Exception e)
            {
                logger.Error("Exception occurred while drawing button '{0}'!\n{1}", btn.Id, e);
            }
        }

        /// <remarks>
        /// <see cref="DrawableButton"/> has a status property, but grid drawer should be able to override this, 
        /// so therefore the <paramref name="status"/> parameter.
        /// </remarks>
        protected abstract void Draw(Graphics g, DrawableButton btn, ButtonStatus status);

        /// <summary>
        /// Extending classes should override this method if necessary, but don't forget to call base method!
        /// </summary>
        public virtual void Dispose()
        {
            drawer.Dispose();  // dispose decorated drawer
        }
    }
}
