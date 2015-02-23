using NLog;
using System;
using System.Windows.Forms;

namespace Player.Core.Element
{
    // TODO A: DevSettings Anchor
    static class KeyboardBuilderFactory
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private static readonly int DEV_KBD_ID = 2;

        public static readonly string HARD_CODED_KEYBOARD = String.Empty; //@"<path>";  // TODO B4PUSH: set to empty string (≙ hard coded keyboard)


        public static KeyboardBuilder CreateKeyboardBuilder(Form form)
        {
            //return CreateKeyboardDevBuilder(form);  // TODO B4PUSH: comment this line (≙ dev keyboard)
            return CreateKeyboardFileBuilder(form);
        }

        private static KeyboardBuilder CreateKeyboardDevBuilder(Form form)
        {
            logger.Debug("Creating KeyboardDevBuilder({0})...", DEV_KBD_ID);
            
            return new KeyboardDevBuilder(DEV_KBD_ID);
        }

        private static KeyboardBuilder CreateKeyboardFileBuilder(Form form)
        {
            logger.Debug("Creating KeyboardFileBuilder...");

            return new KeyboardFileBuilder(form);
        }
    }
}
