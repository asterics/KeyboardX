using NLog;
using Player.Model;
using System;

namespace Player.Core.Scan
{
    /// <summary>
    /// For dev purposes only.<para />
    /// Current implementation does simply scan rows and ignores triggers.
    /// </summary>
    class TestScanner : BaseScanner
    {
        public static readonly string ScannerTypeKeyword = "test";

        private static Logger logger = LogManager.GetCurrentClassLogger();

        
        private int y;


        public TestScanner(GridModel grid, int initialScanDelay, int postAcceptanceDelay, int postInputAcceptanceTime, int scanTime)
            : base(grid, initialScanDelay, postAcceptanceDelay, postInputAcceptanceTime, scanTime)
        {
            // NOP
        }


        public override void OnTrigger()
        {
            logger.Debug("Ignoring trigger, because {0} is not capable of it.", GetType().Name);
        }

        protected override void Reset()
        {
            base.Reset();

            scanState = Scan;
            y = -1;
        }

        protected void Scan(bool trigger = false)
        {
            ButtonGroup selection;
            do
            {
                y = (y + 1) % grid.Rows;
                selection = grid.GetButtonsForRow(y);
            }
            while (selection.IsEmpty());

            logger.Debug("Selecting row {0} of {1}...", y, grid.Rows);
            SelectButtons(selection);
        }
    }
}
