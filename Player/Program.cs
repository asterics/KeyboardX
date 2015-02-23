using Player.Core;
using System;
using System.Windows.Forms;

namespace Player
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                Controller controller = new Controller();
                Application.Run(controller.Form);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while initializing! For more details look at the log file.");
                Console.WriteLine(e);
            }
        }
    }
}
