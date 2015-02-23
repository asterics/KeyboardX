using NLog;
using Player.Model;
using System;

namespace Player.Core.Scan
{
    /// <summary>
    /// Base class for <see cref="ColumnRowScanner"/> and <see cref="RowColumnScanner"/>.
    /// </summary>
    /// TODOs:
    ///  TODO 4: when a whole row or column is empty it's not detected
    abstract class ColumnRowBaseScanner : BaseScanner
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /* config */

        protected readonly bool startTop;
        protected readonly bool startLeft;
        protected readonly int localCycleLimit;

        /* state */

        protected readonly int initialX;
        protected readonly int initialY;

        protected int x, y;
        protected int localCycleCount;

        protected ButtonModel CurButton
        {
            get { return grid[x, y]; }
        }


        public ColumnRowBaseScanner(GridModel grid, int initialScanDelay, int postAcceptanceDelay, int postInputAcceptanceTime, int scanTime, 
                bool startTop, bool startLeft, int localCycleLimit)
            : base(grid, initialScanDelay, postAcceptanceDelay, postInputAcceptanceTime, scanTime)
        {
            this.startTop = startTop;
            this.startLeft = startLeft;
            this.localCycleLimit = localCycleLimit;

            initialX = (startLeft ? -1 : grid.Cols);
            initialY = (startTop ? -1 : grid.Rows);
        }


        protected override void Reset()
        {
            base.Reset();

            x = initialX;
            y = initialY;
            scanState = GlobalScanning;
        }

        protected abstract void GlobalScanning(bool trigger);

        protected abstract void LocalScanning(bool trigger);
    }
}
