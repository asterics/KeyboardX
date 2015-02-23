using NLog;
using Player.Core.Status;
using Player.Draw.Element;
using System;
using System.Drawing;

namespace Player.Draw.Button
{
    class SimpleTextDrawer : ButtonBaseDrawer, IDisposable
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();


        private Font font;
        private Font fontSelected;
        private StringFormat format;


        public SimpleTextDrawer()
        {
            Init();
        }

        public SimpleTextDrawer(ButtonBaseDrawer drawerToDecorate)
            : base(drawerToDecorate)
        {
            Init();
        }


        public override void Dispose()
        {
            base.Dispose();

            if (font != null) font.Dispose();
            if (fontSelected != null) fontSelected.Dispose();
            if (format != null) format.Dispose();
        }

        protected override void Draw(Graphics g, DrawableButton btn, ButtonStatus status)
        {
            if (btn.Text == null || String.IsNullOrWhiteSpace(btn.Text.Content))
                return;

            if (status.Selected)
                g.DrawString(btn.Text.Content, fontSelected, Brushes.Red, btn.PixelPosition, format);
            else
                g.DrawString(btn.Text.Content, font, Brushes.Black, btn.PixelPosition, format);
        }

        private void Init()
        {
            font = new Font(FontFamily.GenericMonospace, 24f);
            fontSelected = new Font(FontFamily.GenericMonospace, 28f, FontStyle.Bold);
            format = new StringFormat();
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;
        }
    }
}
