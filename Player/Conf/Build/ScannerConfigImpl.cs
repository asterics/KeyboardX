using System;

namespace Player.Conf.Build
{
    class ScannerConfigImpl : ScannerConfig
    {
        public string ScannerType { get; set; }

        public bool ScannerActive { get; set; }


        public int InitialScanDelay { get; set; }

        public int InputAcceptanceTime { get; set; }

        public int PostAcceptanceDelay { get; set; }

        public int PostInputAcceptanceTime { get; set; }

        public int RepeatAcceptanceDelay { get; set; }

        public int RepeatTime { get; set; }

        public int ScanTime { get; set; }


        public bool StartLeft { get; set; }

        public bool StartTop { get; set; }

        public bool MoveHorizontal { get; set; }


        public int LocalCycleLimit { get; set; }
        

        public ScannerConfigImpl()
        {
            /* hard coded default values go here */

            ScannerType = "row-column";
            ScannerActive = true;

            InitialScanDelay = 800;
            InputAcceptanceTime = 100;
            PostAcceptanceDelay = 300;
            PostInputAcceptanceTime = 2000;
            RepeatAcceptanceDelay = 500;
            RepeatTime = 250;
            ScanTime = 2000;

            StartLeft = true;
            StartTop = true;
            MoveHorizontal = true;

            LocalCycleLimit = 2;
        }
    }
}
