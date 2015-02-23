using NLog;
using Player.Conf;
using Player.Core.Status;
using Player.Draw.Button;
using Player.Model;
using System.Collections.Generic;
using System.Drawing;

namespace Player.Draw.Grid
{
    /// <summary>
    /// Draws grid of buttons with optional border. The borders of two adjacent buttons are overlapping.
    /// E.g.: <c>border button border button border button border</c>...
    /// </summary>
    /// TODOs:
    ///  TODO 5: [performance] don't construct border rectangles for every button on every draw
    ///  TODO 4: [performance] add GridCachedBorderDrawer which caches all drawn buttons in a bitmap
    class GridBorderDrawer : GridBaseDrawer
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();


        protected int borderWidth;


        public GridBorderDrawer(GridModel grid, GridStatus status, ButtonBaseDrawer drawer)
            : base(grid, status, drawer)
        {
            borderWidth = grid.Style.BorderWidth.Value;
        }

        protected override void DrawGrid(Graphics g)
        {
            List<Rectangle> borderRects = new List<Rectangle>();
            List<Rectangle> selectRects = new List<Rectangle>();
            List<Rectangle> mouseRects = new List<Rectangle>();

            foreach (var btn in grid)
            {
                drawer.DrawButton(g, btn);

                if (borderWidth > 0) 
                {
                    Rectangle[] rects = CreateBorderRects(btn.PixelPosition);
                    if (btn.Status.MouseOver)
                        mouseRects.AddRange(rects);
                    else if (btn.Status.Selected)
                        selectRects.AddRange(rects);
                    else
                        borderRects.AddRange(rects);
                }
            }

            if (borderWidth > 0)
            {
                if (borderRects.Count > 0)
                    g.FillRectangles(grid.BorderBrush, borderRects.ToArray());
                if (selectRects.Count > 0)
                    g.FillRectangles(grid.SelectBrush, selectRects.ToArray());
                if (mouseRects.Count > 0)
                    g.FillRectangles(grid.MouseBrush, mouseRects.ToArray());
            }
        }

        protected override void ResizeGrid()
        {
            int totalBorderWidth = (grid.Cols + 1) * borderWidth;
            int totalBorderHeight = (grid.Rows + 1) * borderWidth;

            int elementWidth = (CanvasSize.Width - totalBorderWidth) / grid.Cols;
            int elementHeight = (CanvasSize.Height - totalBorderHeight) / grid.Rows;

            int gridWidth = grid.Cols * elementWidth + totalBorderWidth;
            int gridHeight = grid.Rows * elementHeight + totalBorderHeight;

            int leftDelta = (CanvasSize.Width - gridWidth) / 2;
            int topDelta = (CanvasSize.Height - gridHeight) / 2;

            foreach (var btn in grid)
            {
                Rectangle rect = new Rectangle();
                ButtonPosition pos = btn.LogicalPosition;
                rect.X = leftDelta + pos.X * (elementWidth + borderWidth) + borderWidth;
                rect.Y = topDelta + pos.Y * (elementHeight + borderWidth) + borderWidth;
                rect.Width = pos.DimX * (elementWidth + borderWidth) - borderWidth;
                rect.Height = pos.DimY * (elementHeight + borderWidth) - borderWidth;

                btn.PixelPosition = rect;
                logger.Trace("Set pixel position of button '{0}' to {1}.", btn.Id, btn.PixelPosition);
            }
        }

        protected Rectangle[] CreateBorderRects(Rectangle btnRect)
        {
            Rectangle left = new Rectangle();
            left.X = btnRect.X - borderWidth;
            left.Y = btnRect.Y - borderWidth;
            left.Width = borderWidth;
            left.Height = btnRect.Height + 2 * borderWidth;

            Rectangle right = new Rectangle();
            right.X = btnRect.Right;
            right.Y = left.Y;
            right.Width = borderWidth;
            right.Height = left.Height;

            Rectangle top = new Rectangle();
            top.X = left.X;
            top.Y = left.Y;
            top.Width = btnRect.Width + 2 * borderWidth;
            top.Height = borderWidth;

            Rectangle bottom = new Rectangle();
            bottom.X = top.X;
            bottom.Y = btnRect.Bottom;
            bottom.Width = top.Width;
            bottom.Height = borderWidth;

            return new Rectangle[] {left, top, right, bottom};
        }
    }
}
