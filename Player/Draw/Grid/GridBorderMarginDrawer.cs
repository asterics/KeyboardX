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
    /// Draws grid of buttons with optional border and margin.<para />
    /// A margin is the space between the border of an element and the margin of a neighbor element or the space between the border of an element 
    /// and the window border.<para />
    /// E.g.: <c>margin border button border margin margin border button border margin</c>...
    /// </summary>
    class GridBorderMarginDrawer : GridBorderDrawer
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        protected int marginWidth;


        public GridBorderMarginDrawer(GridModel grid, GridStatus status, ButtonBaseDrawer drawer)
            : base(grid, status, drawer)
        {
            marginWidth = grid.Style.MarginWidth.Value;
        }


        protected override void ResizeGrid()
        {
            // waste ≈ border + margin
            int wasteWidth = marginWidth + borderWidth;
            int doubleWasteWidth = 2 * wasteWidth;
            int totalWasteWidth = grid.Cols * doubleWasteWidth;
            int totalWasteHeight = grid.Rows * doubleWasteWidth;

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
                rect.X = leftDelta + pos.X * (elementWidth + doubleWasteWidth) + wasteWidth;
                rect.Y = topDelta + pos.Y * (elementHeight + doubleWasteWidth) + wasteWidth;
                rect.Width = pos.DimX * (elementWidth + doubleWasteWidth) - doubleWasteWidth;
                rect.Height = pos.DimY * (elementHeight + doubleWasteWidth) - doubleWasteWidth;

                btn.PixelPosition = rect;
                logger.Trace("Set pixel position of button '{0}' to {1}.", btn.Id, btn.PixelPosition);
            }
        }
    }
}
