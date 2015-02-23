using NLog;
using Player.Model;
using System;

namespace Player.Load.Element
{
    class Icon : ButtonIcon
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();


        public string IconPath { get; set; }

        public DisplayMode DisplayMode { get; private set; }

        public Rotation Rotation { get; private set; }


        public void SetDisplayMode(string mode)
        {
            DisplayMode m = (DisplayMode)Enum.Parse(typeof(DisplayMode), mode, true);
            DisplayMode = m;
        }
    }
}
