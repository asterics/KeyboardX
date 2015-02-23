using Player.Model;
using System;

namespace Player.Load.Element
{
    class ScannerParam : ScannerParameter, KeyboardElement
    {
        public bool? ScannerActive { get; set; }

        public string ScannerType { get; set; }


        public int? InitialScanDelay { get; set; }

        public int? InputAcceptanceTime { get; set; }

        public int? PostAcceptanceDelay { get; set; }

        public int? PostInputAcceptanceTime { get; set; }

        public int? ScanTime { get; set; }


        public bool? StartLeft { get; set; }

        public bool? StartTop { get; set; }

        public bool? MoveHorizontal { get; set; }


        public int? LocalCycleLimit { get; set; }


        public ScannerParam()
        {
            // NOP
        }


        public void Validate()
        {
            // TODO 5: implement validate for SannerParam
            throw new NotImplementedException();
        }
    }
}
