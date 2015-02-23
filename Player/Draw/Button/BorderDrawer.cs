using NLog;
using Player.Core.Status;
using Player.Draw.Element;
using System;
using System.Drawing;

namespace Player.Draw.Button
{
    /// <summary>
    /// Draws border around a button.
    /// </summary>
    class BorderDrawer : ButtonBaseDrawer, IDisposable
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();


        private int normalWidth;

        private int selectedWidth;

        private Pen normalPen;

        private Pen selectedPen;


        public BorderDrawer(ButtonBaseDrawer drawerToDecorate)
            :base(drawerToDecorate)
        {
            normalWidth = 3;
            selectedWidth = normalWidth;
            normalPen = new Pen(Color.Blue, normalWidth);
            selectedPen = new Pen(Color.Red, selectedWidth);
        }


        public override void Dispose()
        {
            base.Dispose();

            if (normalPen != null)
                normalPen.Dispose();

            if (selectedPen != null)
                selectedPen.Dispose();
        }

        protected override void Draw(Graphics g, DrawableButton btn, ButtonStatus status)
        {
            Pen p = (status.Selected ? selectedPen : normalPen);

            Rectangle rect = btn.PixelPosition;  // allowed cause of value type
            int width = (status.Selected ? selectedWidth : normalWidth);
            width--;
            rect.X += width;
            rect.Width -= 2 * width;
            rect.Y += width;
            rect.Height -= 2 * width;

            g.DrawRectangle(p, rect);
        }
    }
}
