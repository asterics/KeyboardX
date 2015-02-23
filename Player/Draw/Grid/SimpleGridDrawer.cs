using NLog;
using Player.Core.Status;
using Player.Draw.Button;
using Player.Draw.Element;
using Player.Model;
using System.Drawing;

namespace Player.Draw.Grid
{
    /// <summary>
    /// Simply draws grid buttons in a loop, no border, no margin.
    /// </summary>
    class SimpleGridDrawer : GridBaseDrawer
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();


        public SimpleGridDrawer(GridModel grid, GridStatus status, ButtonBaseDrawer drawer)
            : base(grid, status, drawer) 
        {
            // NOP
        }


        protected override void DrawGrid(Graphics g)
        {
            foreach (var btn in grid)
                drawer.DrawButton(g, btn);
        }

        protected override void ResizeGrid()
        {
            int colWidth = CanvasSize.Width / grid.Cols;
            int rowHeight = CanvasSize.Height / grid.Rows;

            foreach (var btn in grid)
            {
                Rectangle rect = new Rectangle();
                ButtonPosition pos = btn.LogicalPosition;
                rect.X = pos.X * colWidth;
                rect.Y = pos.Y * rowHeight;
                rect.Width = pos.DimX * colWidth;
                rect.Height = pos.DimY * rowHeight;

                btn.PixelPosition = rect;
                logger.Trace("Set pixel position of button '{0}' to {1}.", btn.Id, btn.PixelPosition);
            }
        }
    }
}
