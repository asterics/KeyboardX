using NLog;
using Player.Core.Gui;
using Player.Core.Status;
using Player.Draw.Button;
using Player.Draw.Grid;
using Player.Model;
using System;

namespace Player.Draw
{
    /// <summary>
    /// This is the only dependency to implementation that should exist to this namespace from outside.
    /// </summary>
    static class DrawerFactory
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();


        public static Drawer CreateDrawer(GridModel grid, GridStatus status)
        {
            //return CreateDevDrawer(grid, status); // TODO B4RELEASE: create the right drawer

            ButtonBaseDrawer buttonDrawer = new AlignedDrawer();

            string type = grid.Style.DrawerType;
            switch (type)
            {
                case "border":          return new GridBorderDrawer(grid, status, buttonDrawer);
                case "border-gap":      return new GridBorderGapDrawer(grid, status, buttonDrawer);
                case "border-margin":   return new GridBorderMarginDrawer(grid, status, buttonDrawer);
                default:
                    throw new Exception(String.Format("Drawer of type '{0}' is not implemented!", type));
            }
        }

        private static Drawer CreateDevDrawer(GridModel grid, GridStatus status)
        {
            logger.Debug("Creating DevelopmentDrawer...");

            ButtonBaseDrawer drawer = new AlignedDrawer();
            return new GridBorderDrawer(grid, status, drawer);
            //return new GridBorderGapDrawer(grid, status, drawer);
            //return new GridBorderMarginDrawer(grid, status, drawer);
        }

        private static Drawer CreateBorderDrawer(GridModel grid, GridStatus status)
        {
            logger.Debug("Creating BorderDrawer consisting of: AlignedDrawer, GridBorderDrawer");

            ButtonBaseDrawer drawer = new AlignedDrawer();
            return new GridBorderDrawer(grid, status, drawer);
        }

        private static Drawer CreateInvertDrawer(GridModel grid, GridStatus status)
        {
            logger.Warn("TODO: create honest invert drawer!");
            return CreateDrawer(grid, status);
        }

        private static Drawer CreateSimpleTextDrawer(GridModel grid, GridStatus status)
        {
            logger.Debug("Creating SimpleTextDrawer consisting of: SimpleTextDrawer, SimpleGridDrawer");

            ButtonBaseDrawer drawer = new SimpleTextDrawer();
            return new SimpleGridDrawer(grid, status, drawer);
        }

        private static Drawer CreateIconDrawer(GridModel grid, GridStatus status)
        {
            logger.Debug("Creating IconDrawer consisting of: SimpleIconDrawer, SimpleGridDrawer");

            ButtonBaseDrawer drawer = new SimpleIconDrawer();
            return new SimpleGridDrawer(grid, status, drawer);
        }

        private static Drawer CreateAlignedDrawer(GridModel grid, GridStatus status)
        {
            logger.Debug("Creating AlignedDrawer consisting of: BackgroundColorDrawer, AlignedDrawer, SimpleGridDrawer");

            ButtonBaseDrawer drawer;
            drawer = new AlignedDrawer();
            drawer = new BorderDrawer(drawer);
            return new SimpleGridDrawer(grid, status, drawer);
        }
    }
}
