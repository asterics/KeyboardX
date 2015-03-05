using NLog;
using Player.Load;
using Player.Model;
using Player.Util;
using System;
using System.IO;
using System.Windows.Forms;

namespace Player.Core.Element
{
    /// <summary>
    /// Builds a <see cref="Keyboard"/> object from a file.
    /// </summary>
    /// <remarks>
    /// A reference to <see cref="Form"/> is needed for dialogs and such.
    /// </remarks>
    class KeyboardFileBuilder : KeyboardBuilder
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private readonly Form form;


        public KeyboardFileBuilder(Form form)
        {
            this.form = form;
        }


        public Keyboard BuildKeyboard()
        {
            KeyboardModel model = OpenKeyboardModel();
            try { return new Keyboard(model); }
            catch (Exception e)
            {
                string msg = "Exception occurred while building keyboard form model!";
                logger.Error(ExceptionUtil.Format(msg, e));
                throw new Exception(msg);
            }
        }

        /// <summary>
        /// Opens a keyboard file and afterwards loads a <see cref="KeyboardModel"/>.<para/>
        /// The priority for opening a keyboard is as follows:
        ///   1. hard coded path
        ///   2. command line argument
        ///   3. open file dialog
        /// </summary>
        /// <exception cref="Exception">If any error occurs while loading keyboard model or no keyboard file was provided.</exception>
        private KeyboardModel OpenKeyboardModel()
        {
            string path = null;

            /* hard coded path */

            path = KeyboardBuilderFactory.HARD_CODED_KEYBOARD;
            if (!String.IsNullOrWhiteSpace(path))
            {
                if (File.Exists(path))
                {
                    logger.Debug("Loading keyboard from hard coded path... ({0})", path);
                    return LoadKeyboardModel(path);
                }
                else
                    logger.Warn("No keyboard file found on hard coded path (\"{0}\")!", path);
            }

            /* command line argument */

            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                path = args[1];
                if (File.Exists(path))
                {
                    logger.Debug("Loading keyboard from command line argument... ({0})", path);
                    return LoadKeyboardModel(path);
                }
                else
                    logger.Error("No keyboard file found on given argument path (\"{0}\")!", path);
            }

            /* open file dialog */

            path = GetPathFromFileDialog();
            if (File.Exists(path))
            {
                logger.Debug("Loading keyboard from open file dialog... ({0})", path);
                return LoadKeyboardModel(path);
            }

            throw new Exception("No keyboard file was provided.");
        }

        private string GetPathFromFileDialog()
        {
            string path = null;

            MethodInvoker showDialog = delegate()
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Title = "Open Keyboard";
                ofd.Filter = "Keyboard File|*.xml";
                ofd.ShowDialog();
                path = ofd.FileName;
            };

            if (form.InvokeRequired)
                form.Invoke(showDialog);
            else
                showDialog();

            return path;
        }

        private KeyboardModel LoadKeyboardModel(string path)
        {
            try
            {
                Loader ldr = LoaderFactory.CreateLoader();
                return ldr.LoadKeyboard(path);
            }
            catch (LoaderException e)
            {
                string msg = e.Message;
                if (e.InnerException != null)
                {
                    msg += "\n" + e.InnerException.Message;
                }
                logger.Warn("Exception occurred while loading keyboard!\n{0}", msg);
                throw new Exception(msg);
            }
            catch (Exception e)
            {
                string msg = "Unrecognized exception occurred while loading keyboard!";
                logger.Error(ExceptionUtil.Format(msg, e));
                throw new Exception(msg);
            }
        }
    }
}
