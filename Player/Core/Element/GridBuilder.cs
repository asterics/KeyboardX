using NLog;
using Player.Conf;
using Player.Core.Gui;
using Player.Core.Scan;
using Player.Core.Status;
using Player.Draw;
using Player.Model;
using System;

namespace Player.Core.Element
{
    /// <summary>
    /// Helper class for building a <see cref="Player.Core.Element.Grid"/> object.
    /// </summary>
    class GridBuilder
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();


        public static Grid BuildGrid(GridModel model, ScannerParameter kbScanParams, GeneralStyle kbStyle)
        {
            InheritStyleTillButtons(model, kbStyle);

            GridStatusImpl status = new GridStatusImpl(model);

            Drawer drawer = BuildDrawer(model, status);

            Scanner scanner = BuildScanner(model, kbScanParams);

            return new Grid(model, status, drawer, scanner);
        }

        private static void InheritStyleTillButtons(GridModel model, GeneralStyle kbStyle)
        {
            if (model.Style == null)
                model.Style = kbStyle.GetShallowCopy();
            else
                model.Style.InheritFrom(kbStyle);

            foreach (var btn in model)
            {
                if (btn.Style == null)
                    btn.Style = model.Style.GetShallowCopy();
                else
                    btn.Style.InheritFrom(model.Style);
            }
        }

        private static Drawer BuildDrawer(GridModel grid, GridStatus status)
        {
            try { return DrawerFactory.CreateDrawer(grid, status); }
            catch (Exception e)
            {
                string msg = String.Format("Building drawer for grid '{0}' failed!", grid.Id);
                throw new Exception(msg, e);
            }
        }

        private static Scanner BuildScanner(GridModel grid, ScannerParameter kbScanParams)
        {
            try { return ScannerFactory.CreateScanner(grid, kbScanParams); }
            catch (Exception e)
            {
                logger.Error("Building scanner for grid '{0}' failed!\n{1}", grid.Id, e.Message);
                return null;
            }
        }
    }
}
