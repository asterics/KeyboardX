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
    class RowColumnScanner : ColumnRowBaseScanner
    {
        public static readonly string ScannerTypeKeyword = "row-column";

        private static Logger logger = LogManager.GetCurrentClassLogger();


        /// <summary>The maximum column index for current grid (zero based of course).</summary>
        private readonly int maxCol;


        public RowColumnScanner(GridModel grid, int initialScanDelay, int postAcceptanceDelay, int postInputAcceptanceTime, int scanTime,
                bool startTop, bool startLeft, int localCycleLimit)
            : base(grid, initialScanDelay, postAcceptanceDelay, postInputAcceptanceTime, scanTime, startTop, startLeft, localCycleLimit)
        {
            maxCol = grid.Cols - 1;
        }


        # region RowScanning

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
                if (SwitchedToLocalBecauseOfRowCount())
                    return;

                SelectNextRow();
            }
        }

        private bool ButtonPressedBecauseOfButtonCount()
        {
            if (grid.GetButtonsForRow(y).Count > 1)
                return false;
            else
            {
                logger.Trace("Current row contains only 1 button, so column scanning makes no sense.");

                x = initialX;
                MoveToNextButton();
                PressButton(CurButton);
                y = initialY;

                return true;
            }

        }

        private bool SwitchedToLocalBecauseOfRowCount()
        {
            if (grid.NonEmptyRows > 1)
                return false;
            else
            {
                logger.Trace("Grid has only 1 (nonempty) row, so row scanning makes no sense.");

                MoveToNextNonEmptyRow();
                SwitchToLocalScanning();

                return true;
            }
        }

        private void SwitchToLocalScanning()
        {
            logger.Trace("Switching to local scanning (column scanning)...");

            x = initialX;
            localCycleCount = 0;
            scanState = LocalScanning;
            scanState(false);
        }

        private void SelectNextRow()
        {
            ButtonGroup row = MoveToNextNonEmptyRow();
            logger.Trace("Selecting row {0}...", y);
            SelectButtons(row);
        }

        private ButtonGroup MoveToNextNonEmptyRow()
        {
            ButtonGroup row;
            do
            {
                MoveToNextRow();
                row = grid.GetButtonsForRow(y);
            }
            while (row.IsEmpty());

            return row;
        }

        /// <summary>Moves internal row pointer to next row depending on settings.</summary>
        private void MoveToNextRow()
        {
            int maxRow = grid.Rows - 1;

            if (startTop)
                y = (y == maxRow ? 0 : y + 1);
            else
                y = (y == 0 ? maxRow : y - 1);
        }

        # endregion

        # region ColumnScanning

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
            logger.Trace("Switching back to global scanning (row scanning)...");

            y = initialY;
            scanState = GlobalScanning;
            scanState(false);
        }

        private void SelectNextButton()
        {
            MoveToNextButton();

            if (localCycleCount >= localCycleLimit)
            {
                logger.Debug("Reached column cycle limit ({0}).", localCycleLimit);
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
            if (x >= 0 && x <= maxCol)
                prev = CurButton;

            do  // move to next until a different button is reached
            {
                if (startLeft)
                    MoveRight();
                else
                    MoveLeft();
            }
            while (CurButton == null || CurButton == prev);
        }

        private void MoveRight()
        {
            if (x == maxCol)
            {
                x = 0;
                localCycleCount++;
            }
            else
                x++;
        }

        private void MoveLeft()
        {
            if (x == 0)
            {
                x = maxCol;
                localCycleCount++;
            }
            else
                x--;
        }

        #endregion
    }
}
