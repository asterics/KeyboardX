using NLog;
using Player.Conf;
using Player.Load.Element;
using Player.Load.Parse;
using Player.Model;
using Player.Util;
using System;
using System.IO;

namespace Player.Load
{
    /// <summary>
    /// Loads a keyboard from a given path.
    /// </summary>
    class KeyboardLoader : Loader
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();


        public KeyboardModel LoadKeyboard(string path)
        {
            SetKeyboardsBaseDirectory(path);

            try
            {
                using (Stream keyboardStream = OpenKeyboard(path))
                {
                    if (Config.Global.SchemaValidation)
                        ValidateKeyboardBySchema(keyboardStream);

                    Keyboard kb = ParseKeyboard(keyboardStream);
                    kb.Name = Path.GetFileName(path);
                    kb.Validate();

                    return kb;
                }
            }
            catch (LoaderException e)
            {
                logger.Error(ExceptionUtil.Format(e));
                throw;
            }
            catch (Exception e)
            {
                string msg = "Unrecognized exception occurred while loading keyboard!";
                logger.Error(ExceptionUtil.Format(msg, e));
                throw new LoaderException(msg, e);
            }
        }

        // TODO 2: [performance] check if MemoryStream is possible instead FileStream and if it brings performance boost
        private Stream OpenKeyboard(string path)
        {
            FileStream fs = null;

            try
            {
                fs = new FileStream(path, FileMode.Open);
            }
            catch (Exception e)
            {
                string msg = "Opening keyboard file failed!";
                throw new LoaderException(msg, e);
            }

            return fs;
        }

        private void ValidateKeyboardBySchema(Stream keyboardStream)
        {
            logger.Debug("Validating keyboard by schema...");

            XmlSchemaValidator val = new XmlSchemaValidator();
            val.Validate(keyboardStream);

            if (!val.IsValid())
            {
                string msg = String.Format("Errors or warnings occurred while validating keyboard with schema file!\n{0}", val.GetErrorsAndWarnings());
                throw new LoaderException(msg);
            }
        }

        private Keyboard ParseKeyboard(Stream keyboardStream)
        {
            logger.Debug("Parsing keyboard...");

            keyboardStream.Position = 0;  // important, because stream is read 2nd time

            KeyboardParser p = new KeyboardParser();
            return p.ParseKeyboard(keyboardStream);
        }

        // TODO 2: [zip] remove this block as soon as it's not needed anymore
        private void SetKeyboardsBaseDirectory(string path)
        {
            try
            {
                Config.KeyboardsBaseDirectory = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(path), ".."));
                Config.IconBaseDirectory = Path.Combine(Config.KeyboardsBaseDirectory, "icons");
            }
            catch (Exception e)
            {
                string msg = String.Format("{0} occurred while setting KeyboardsBaseDirectory!", e.GetType().Name);
                logger.Error(ExceptionUtil.Format(msg, e));
                throw new LoaderException(msg, e);
            }
        }
    }
}
