using NLog;
using Player.Core.Status;
using Player.Draw.Element;
using Player.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Player.Draw.Button
{
    /// <summary>
    /// Draws icon and text of a button considering display mode and alignment.
    /// </summary>
    class ButtonAlignedDrawer : ButtonBaseDrawer, IDisposable
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();


        protected Font fontDefault;

        protected Font fontSelected;

        protected Dictionary<TextAlignment, StringFormat> alignedFormats;


        public ButtonAlignedDrawer()
        {
            Init();
        }

        public ButtonAlignedDrawer(ButtonBaseDrawer drawerToDecorate)
            :base(drawerToDecorate)
        {
            Init();
        }


        public override void Dispose()
        {
            base.Dispose();

            if (fontDefault != null)
                fontDefault.Dispose();

            if (fontSelected != null)
                fontSelected.Dispose();
        }

        protected override void Draw(Graphics g, DrawableButton btn, ButtonStatus status)
        {
            g.FillRectangle(btn.BackBrush, btn.PixelPosition);

            if (btn.Icon != null)
            {
                if (btn.Text != null)
                {
                    Rectangle alignRect = GetAlignedIconRect(btn.Text.Alignment, btn.PixelPosition);
                    DrawIcon(g, btn.Icon, alignRect);
                }
                else
                {
                    DrawIcon(g, btn.Icon, btn.PixelPosition);
                }
            }

            if (btn.Text != null)
            {
                DrawText(g, btn.Text, btn.FontBrush, btn.PixelPosition);
            }
        }

        protected Rectangle GetAlignedIconRect(TextAlignment txtAlign, Rectangle btnRect)
        {
            if (txtAlign == TextAlignment.Center)
            {
                return btnRect;
            }

            RectangleF iconRect = btnRect;
            float h = btnRect.Height / 3;

            if (txtAlign == TextAlignment.Top)
            {
                iconRect.Y += h;
                iconRect.Height = 2 * h;
            }
            else if (txtAlign == TextAlignment.Bottom)
            {
                iconRect.Height = 2 * h;
            }
            else
            {
                string msg = String.Format("Alignment '{0}' is not implemented!", txtAlign.ToString().ToLower());
                throw new ApplicationException(msg);
            }

            return Rectangle.Round(iconRect);  // TODO 2: check if this rounding yields correct behavior
        }

        protected void DrawIcon(Graphics g, DrawableIcon icn, Rectangle btnRect)
        {
            try
            {
                switch (icn.DisplayMode)
                {
                    case DisplayMode.Normal:
                        DrawImageNormal(g, icn.Image, btnRect);
                        break;
                    case DisplayMode.Stretch:
                        g.DrawImage(icn.Image, btnRect);
                        break;
                    case DisplayMode.Zoom:
                        DrawImageZoomed(g, icn.Image, btnRect);
                        break;
                    default:
                        string msg = String.Format("Display mode '{0}' is not implemented!", icn.DisplayMode.ToString().ToLower());
                        throw new ApplicationException(msg);
                }
            }
            catch (ApplicationException e)
            {
                logger.Warn(e.Message);
            }
        }

        protected void DrawImageNormal(Graphics g, Image img, Rectangle btnRect)
        {
            RectangleF imgRect = btnRect;
            float btnRatio = btnRect.Width / btnRect.Height;
            float imgRatio = (img.Width * 1f) / img.Height;

            // TODO 3: special case if ratio is equal
            if (btnRatio < imgRatio)  // adapt height
            {
                imgRect.Height = btnRect.Width / imgRatio;
                imgRect.Y += (btnRect.Height - imgRect.Height) * 0.5f;
            }
            else  // adapt width
            {
                imgRect.Width = btnRect.Height * imgRatio;
                imgRect.X += (btnRect.Width - imgRect.Width) * 0.5f;
            }

            g.DrawImage(img, imgRect);
        }

        protected void DrawImageZoomed(Graphics g, Image img, Rectangle btnRect)
        {
            RectangleF srcRect = new RectangleF();
            float btnRatio = btnRect.Width / btnRect.Height;
            float imgRatio = (img.Width * 1f) / img.Height;

            // TODO 3: special case if ratio is equal
            if (btnRatio < imgRatio)  // full height, clip width
            {
                srcRect.Height = img.Height;
                srcRect.Width = img.Height * btnRatio;
                srcRect.X = (img.Width - srcRect.Width) * 0.5f;
            }
            else  // full width, clip height
            {
                srcRect.Width = img.Width;
                srcRect.Height = img.Width / btnRatio;
                srcRect.Y = (img.Height - srcRect.Height) * 0.5f;
            }

            g.DrawImage(img, btnRect, srcRect, GraphicsUnit.Pixel);
        }

        protected void DrawText(Graphics g, ButtonText txt, Brush color, Rectangle btnRect)
        {
            string text = txt.Content;
            Font ft = fontDefault;
            StringFormat fm;

            try
            {
                fm = alignedFormats[txt.Alignment];
            }
            catch (KeyNotFoundException)
            {
                logger.Warn("Alignment '{0}' is not implemented in {1}!", txt.Alignment.ToString().ToLower(), GetType().Name);
                return;
            }

            g.DrawString(text, ft, color, btnRect, fm);
        }

        private void Init()
        {
            fontDefault = new Font(FontFamily.GenericMonospace, 24f);
            fontSelected = new Font(FontFamily.GenericMonospace, 28f, FontStyle.Bold);

            StringFormat fm;
            alignedFormats = new Dictionary<TextAlignment, StringFormat>();

            // center
            fm = new StringFormat();
            fm.Alignment = StringAlignment.Center;
            fm.LineAlignment = StringAlignment.Center;
            alignedFormats[TextAlignment.Center] = fm;

            // top
            fm = new StringFormat();
            fm.Alignment = StringAlignment.Center;
            fm.LineAlignment = StringAlignment.Near;
            alignedFormats[TextAlignment.Top] = fm;

            // bottom
            fm = new StringFormat();
            fm.Alignment = StringAlignment.Center;
            fm.LineAlignment = StringAlignment.Far;
            alignedFormats[TextAlignment.Bottom] = fm;
        }
    }
}
