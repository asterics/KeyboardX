using NLog;
using Player.Model;
using Svg;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Player.Draw.Element
{
    /// <summary>
    /// Wraps a <see cref="ButtonIcon"/> instance and holds an image that can be drawn directly.
    /// </summary>
    /// TODOs:
    ///  TODO 3: truly support svg files, currently they are not really scalable
    ///  TODO 1: Image.FromFile(), file is locked until image is disposed
    class DrawableIcon : IDisposable
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();


        protected ButtonIcon model;

        public DisplayMode DisplayMode
        {
            get { return model.DisplayMode; }
        }

        public Image Image { get; protected set; }


        public DrawableIcon(ButtonIcon icn)
        {
            model = icn;
            LoadImage();
        }


        public void Dispose()
        {
            if (Image != null)
                Image.Dispose();
        }

        protected virtual void LoadImage()
        {
            try
            {
                string path = model.IconPath;

                if (path.EndsWith("WMF", StringComparison.OrdinalIgnoreCase))
                {
                    logger.Trace("WMF icon detected ({0})", path);
                    Image = Metafile.FromFile(path);
                }
                else if (path.EndsWith("EMF", StringComparison.OrdinalIgnoreCase))
                {
                    logger.Trace("EMF icon detected ({0})", path);
                    Image = Metafile.FromFile(path);
                }
                else if (path.EndsWith("SVG", StringComparison.OrdinalIgnoreCase))
                {
                    logger.Trace("SVG icon detected ({0})", path);

                    SvgDocument svg = SvgDocument.Open(path);
                    Image = svg.Draw();
                }
                else
                {
                    Image = Image.FromFile(path);
                }
            }
            catch (FileNotFoundException e)
            {
                string msg = String.Format("Icon couldn't be found! ({0})", model.IconPath);
                throw new ApplicationException(msg, e);
            }
            catch (OutOfMemoryException e)
            {
                string msg = String.Format("Icon doesn't have a valid image format! ({0})", model.IconPath);
                throw new ApplicationException(msg, e);
            }
        }
    }
}
