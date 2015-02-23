using System;

namespace Player.Model.Action
{
    class ScannerActionParameter : BaseActionParameter
    {
        public static readonly string ActionTypeKeyword = "scanner";


        public bool Start { get; private set; }

        public ScannerActionParameter(bool start)
        {
            Start = start;
        }
    }
}
