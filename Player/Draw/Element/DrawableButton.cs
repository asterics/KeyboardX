using NLog;
using Player.Core.Status;
using Player.Model;
using Player.Util;
using System;
using System.Drawing;

namespace Player.Draw.Element
{
    /// <summary>
    /// Wraps a ButtonModel instance and extends it with stuff needed for drawing.
    /// </summary>
    class DrawableButton : IDisposable
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();


        protected ButtonModel model;

        public string Id
        {
            get { return model.Id; }
        }

        public ButtonPosition LogicalPosition
        {
            get { return model.Position; }
        }

        public Rectangle PixelPosition { get; set; }

        public Rectangle[] BorderRectangles { get; set; }

        public ButtonText Text
        {
            get { return model.Text; }
        }

        public DrawableIcon Icon { get; protected set; }

        public ButtonStatus Status { get; protected set; }

        public Brush BackBrush { get; protected set; }

        public Brush FontBrush { get; protected set; }


        public DrawableButton(ButtonModel btn, ButtonStatus status)
        {
            model = btn;
            Status = status;
            Init();
        }

        private void Init()
        {
            BackBrush = CreateBrush(model.Style.ButtonBackColor);
            FontBrush = CreateBrush(model.Style.ButtonFontColor);

            if (model.Icon != null)
                InitIcon();
        }

        protected void InitIcon()
        {
            try
            {
                Icon = new DrawableIcon(model.Icon);
            }
            catch (ApplicationException e)
            {
                logger.Error(ExceptionUtil.Format(e));
            }
            catch (NotImplementedException e)
            {
                logger.Error(e);
            }
        }

        public void Dispose()
        {
            if (BackBrush != null) BackBrush.Dispose();
            if (FontBrush != null) FontBrush.Dispose();
            if (Icon != null) Icon.Dispose();
        }

        private Brush CreateBrush(string color)
        {
            Color c = ColorTranslator.FromHtml(color);
            return new SolidBrush(c);
        }
    }
}
