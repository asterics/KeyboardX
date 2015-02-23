using NLog;
using Player.Load;
using Player.Model;
using System;
using System.Windows.Forms;

namespace Player.Core.Element
{
    /// <summary>
    /// Builds a keyboard object from code.
    /// Used only for development.
    /// </summary>
    class KeyboardDevBuilder : KeyboardBuilder
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private readonly int kbdId;


        public KeyboardDevBuilder(int id)
        {
            kbdId = id;
        }


        public Keyboard BuildKeyboard()
        {
            try
            {
                KeyboardModel model = Creator.CreateDevKeyboard(kbdId);
                return new Keyboard(model);
            }
            catch (Exception e)
            {
                logger.Error("Exception occurred while building DevKB{0}!\n{1}", kbdId, e);
                throw;
            }
        }
    }
}
