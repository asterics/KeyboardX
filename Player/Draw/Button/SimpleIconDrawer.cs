using NLog;
using Player.Core.Status;
using Player.Draw.Element;
using System;
using System.Drawing;
using System.IO;

namespace Player.Draw.Button
{
    /// <summary>
    /// Draws the icon of a button in the center.
    /// </summary>
    class SimpleIconDrawer : ButtonBaseDrawer
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();


        public SimpleIconDrawer()
        {
            // NOP
        }

        public SimpleIconDrawer(ButtonBaseDrawer drawerToDecorate)
            : base(drawerToDecorate)
        {
            // NOP
        }


        protected override void Draw(Graphics g, DrawableButton btn, ButtonStatus status)
        {
            if (btn.Icon == null)
                return;

            g.DrawImage(btn.Icon.Image, btn.PixelPosition);
        }
    }
}
