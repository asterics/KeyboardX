using NLog;
using Player.Model;
using System;

namespace Player.Core.Scan
{
    /// <summary>
    /// Scans grid in a linear way. Starting either top-left, top-right, bottom-left or bottom-right, 
    /// moving either vertical or horizontal, cycling through all available buttons.
    /// </summary>
    class LinearScanner : BaseScanner
    {
        public static readonly string ScannerTypeKeyword = "linear";

        private static Logger logger = LogManager.GetCurrentClassLogger();


        /* config */
        private readonly bool startTop;
        private readonly bool startLeft;
        private readonly bool moveHorizontal;

        /* state */
        protected int x, y;

        protected ButtonModel CurButton
        {
            get { return grid[x, y]; }
        }


        public LinearScanner(GridModel grid, int initialScanDelay, int postAcceptanceDelay, int postInputAcceptanceTime, int scanTime,
                bool startTop, bool startLeft, bool moveHorizontal)
            : base(grid, initialScanDelay, postAcceptanceDelay, postInputAcceptanceTime, scanTime)
        {
            this.startTop = startTop;
            this.startLeft = startLeft;
            this.moveHorizontal = moveHorizontal;
        }


        protected override void Reset()
        {
            base.Reset();
            scanState = StartScan;
        }

        protected void StartScan(bool trigger = false)
        {
            x = (startLeft ? grid.Cols - 1 : 0);
            y = (startTop ? grid.Rows - 1 : 0);
            MoveToNext();

            scanState = Scan;
            SelectCurrentButton();
        }

        protected void Scan(bool trigger)
        {
            if (trigger)
            {
                PressButton(CurButton);
                StartScan();
            }
            else
            {
                MoveToNext();
                SelectCurrentButton();
            }
        }

        private void SelectCurrentButton()
        {
            string btnId = CurButton.Id;
            logger.Trace("Selecting button '{0}' at [{1}, {2}]...", btnId, x, y);
            SelectButtons(new ButtonGroup(btnId));
        }

        private void MoveToNext()
        {
            ButtonModel prev = CurButton;

            do
            {
                if (moveHorizontal)
                    MoveHorizontal();
                else
                    MoveVertical();
            }
            while (CurButton == null || CurButton == prev);
        }

        private void MoveHorizontal(bool moved = false)
        {
            if (startLeft)
                MoveRight(moved);
            else
                MoveLeft(moved);
        }

        private void MoveVertical(bool moved = false)
        {
            if (startTop)
                MoveDown(moved);
            else
                MoveUp(moved);
        }

        private void MoveRight(bool moved)
        {
            x++;
            if (x == grid.Cols)  // border reached
            {
                x = 0;
                if (!moved)
                    MoveVertical(true);
            }
        }

        private void MoveLeft(bool moved)
        {
            x--;
            if (x < 0)  // border reached
            {
                x = grid.Cols - 1;
                if (!moved)
                    MoveVertical(true);
            }
        }      

        private void MoveDown(bool moved)
        {
            y++;
            if (y == grid.Rows)  // border reached
            {
                y = 0;
                if (!moved)
                    MoveHorizontal(true);
            }
        }

        private void MoveUp(bool moved)
        {
            y--;
            if (y < 0)  // border reached
            {
                y = grid.Rows - 1;
                if (!moved)
                    MoveHorizontal(true);
            }
        }
    }
}
