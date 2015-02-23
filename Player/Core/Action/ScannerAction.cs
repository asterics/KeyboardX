using NLog;
using Player.Model.Action;
using System;
using ActionDelegate = System.Action;

namespace Player.Core.Action
{
    /// <summary>
    /// Starts or stops the scanner of the current grid.
    /// </summary>
    class ScannerAction : BaseAction<ScannerActionParameter>
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private ActionDelegate scannerAction;


        public ScannerAction(ActionParameter param, ActionDelegate startScanner, ActionDelegate stopScanner)
            : base(param)
        {
            if (Param.Start)
                scannerAction = CheckAction(startScanner);
            else
                scannerAction = CheckAction(stopScanner);
        }


        private ActionDelegate CheckAction(ActionDelegate action)
        {
            if (action == null)
                throw new ArgumentException("Delegate for scanner action may not be null!");
            else
                return action;
        }

        public override void DoAction()
        {
            if (Param.Start)
                logger.Debug("Starting scanner...");
            else
                logger.Debug("Stopping scanner...");

            scannerAction();
        }
    }
}
