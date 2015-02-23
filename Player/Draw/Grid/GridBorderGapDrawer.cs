using NLog;
using Player.Core.Status;
using Player.Draw.Button;
using Player.Model;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Player.Draw.Grid
{
    /// <summary>
    /// Draws grid of buttons with optional border and gap.<para />
    /// A gap is the space between the border of two elements or the space between the border of an element and the window border.<para />
    /// E.g.: <c>gap border button border gap border button border gap</c>...
    /// </summary>
    class GridBorderGapDrawer : GridBorderDrawer
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        protected int gapWidth;


        public GridBorderGapDrawer(GridModel grid, GridStatus status, ButtonBaseDrawer drawer)
            : base(grid, status, drawer)
        {
            gapWidth = grid.Style.GapWidth.Value;
        }


        protected override void ResizeGrid()
        {
            // waste ≈ border + gap
            int betweenWidth = 2 * borderWidth + gapWidth;
            int totalWasteWidth = grid.Cols * betweenWidth + gapWidth;
            int totalWasteHeight = grid.Rows * betweenWidth + gapWidth;

            int elementWidth = (CanvasSize.Width - totalWasteWidth) / grid.Cols;
            int elementHeight = (CanvasSize.Height - totalWasteHeight) / grid.Rows;

            int gridWidth = grid.Cols * elementWidth + totalWasteWidth;
            int gridHeight = grid.Rows * elementHeight + totalWasteHeight;

            int leftDelta = (CanvasSize.Width - gridWidth) / 2;
            int topDelta = (CanvasSize.Height - gridHeight) / 2;

            foreach (var btn in grid)
            {
                Rectangle rect = new Rectangle();
                ButtonPosition pos = btn.LogicalPosition;
                rect.X = leftDelta + pos.X * (elementWidth + betweenWidth) + borderWidth + gapWidth;
                rect.Y = topDelta + pos.Y * (elementHeight + betweenWidth) + borderWidth + gapWidth;
                rect.Width = pos.DimX * (elementWidth + betweenWidth) - betweenWidth;
                rect.Height = pos.DimY * (elementHeight + betweenWidth) - betweenWidth;

                btn.PixelPosition = rect;
                logger.Trace("Set pixel position of button '{0}' to {1}.", btn.Id, btn.PixelPosition);
            }
        }
    }
}
