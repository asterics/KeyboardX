using NLog;
using Player.Model;
using System;

namespace Player.Core.Scan
{
    /// <summary>
    /// A sophisticated row column scanner.
    /// <para />
    /// Is able to start at top/bottom or left/right for row or column scanning. 
    /// Can handle grids with a [1,*], [*,1] and [1,1] dimension.
    /// </summary>
    class ColumnRowScanner : ColumnRowBaseScanner
    {
        public static readonly string ScannerTypeKeyword = "column-row";

        private static Logger logger = LogManager.GetCurrentClassLogger();


        /// <summary>The maximum row index for current grid (zero based of course).</summary>
        private readonly int maxRow;


        public ColumnRowScanner(GridModel grid, int initialScanDelay, int postAcceptanceDelay, int postInputAcceptanceTime, int scanTime,
                bool startTop, bool startLeft, int localCycleLimit)
            : base(grid, initialScanDelay, postAcceptanceDelay, postInputAcceptanceTime, scanTime, startTop, startLeft, localCycleLimit)
        {
            maxRow = grid.Rows - 1;
        }


        # region ColumnScanning

        protected override void GlobalScanning(bool trigger)
        {
            if (trigger)
            {
                if (ButtonPressedBecauseOfButtonCount())
                    return;

                SwitchToLocalScanning();
            }
            else
            {
                if (SwitchedToLocalBecauseOfColumnCount())
                    return;

                SelectNextCol();
            }
        }

        private bool ButtonPressedBecauseOfButtonCount()
        {
            if (grid.GetButtonsForCol(x).Count > 1)
                return false;
            else
            {
                logger.Trace("Current column contains only 1 button, so row scanning makes no sense.");
                
                y = initialY;
                MoveToNextButton();
                PressButton(CurButton);
                x = initialX;
                
                return true;
            }

        }

        private bool SwitchedToLocalBecauseOfColumnCount()
        {
            if (grid.Cols > 1)
                return false;
            else
            {
                logger.Trace("Grid has only 1 column, so column scanning makes no sense.");
                x = 0;
                SwitchToLocalScanning();
                return true;
            }
        }

        private void SwitchToLocalScanning()
        {
            logger.Trace("Switching to local scanning (row scanning)...");

            y = initialY;
            localCycleCount = 0;
            scanState = LocalScanning;
            scanState(false);
        }

        private void SelectNextCol()
        {
            ButtonGroup col;
            do
            {
                MoveToNextCol();
                col = grid.GetButtonsForCol(x);
            }
            while (col.IsEmpty());

            logger.Trace("Selecting column {0}...", x);
            SelectButtons(col);
        }

        /// <summary>Moves internal row pointer to next column depending on settings.</summary>
        private void MoveToNextCol()
        {
            int maxCol = grid.Cols - 1;
            
            if (startLeft)
                x = (x == maxCol ? 0 : x + 1);
            else
                x = (x == 0 ? maxCol : x - 1);
        }

        # endregion

        # region RowScanning

        protected override void LocalScanning(bool trigger)
        {
            if (trigger)
            {
                PressButton(CurButton);
                SwitchToGlobalScanning();
            }
            else
                SelectNextButton();
        }

        private void SwitchToGlobalScanning()
        {
            logger.Trace("Switching back to global scanning (column scanning)...");

            x = initialX;
            scanState = GlobalScanning;
            scanState(false);
        }

        private void SelectNextButton()
        {
            MoveToNextButton();

            if (localCycleCount >= localCycleLimit)
            {
                logger.Debug("Reached row cycle limit ({0}).", localCycleLimit);
                SwitchToGlobalScanning();
                return;
            }

            logger.Trace("Selecting button '{0}' at [{1}, {2}]...", CurButton.Id, x, y);
            SelectButtons(new ButtonGroup(CurButton.Id));
        }

        /// <summary>Moves internal column pointer to next button depending on settings.</summary>
        private void MoveToNextButton()
        {
            ButtonModel prev = null;
            if (y >= 0 && y <= maxRow)
                prev = CurButton;

            do  // move to next until a different button is reached
            {
                if (startTop)
                    MoveDown();
                else
                    MoveUp();
            }
            while (CurButton == null || CurButton == prev);
        }

        private void MoveDown()
        {
            if (y == maxRow)
            {
                y = 0;
                localCycleCount++;
            }
            else
                y++;
        }

        private void MoveUp()
        {
            if (y == 0)
            {
                y = maxRow;
                localCycleCount++;
            }
            else
                y--;
        }

        #endregion
    }
}
