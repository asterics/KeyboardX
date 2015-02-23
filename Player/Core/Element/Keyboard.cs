using NLog;
using Player.Conf;
using Player.Model;
using System;
using System.Collections.Generic;

namespace Player.Core.Element
{
    /// <summary>
    /// Represents a keyboard. Grids are generated lazy.
    /// </summary>
    class Keyboard
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();


        private KeyboardModel model;

        public string Name
        {
            get { return model.Name; }
        }

        private Dictionary<string, Grid> grids;

        public Grid this[string gridId]
        {
            get
            {
                if (grids.ContainsKey(gridId))
                    return grids[gridId];
                else
                    return BuildAndCacheGrid(gridId);
            }
        }

        public Grid DefaultGrid
        {
            get { return this[model.DefaultGrid.Id]; }
        }


        public Keyboard(KeyboardModel model)
        {
            this.model = model;
            grids = new Dictionary<string, Grid>();

            InheritStyleFromApplication();
            BuildAndCacheGrid(model.DefaultGrid.Id);
        }


        private void InheritStyleFromApplication()
        {
            if (model.Style == null)
                model.Style = Config.Style.GetShallowCopy();
            else
                model.Style.InheritFrom(Config.Style);
        }

        private Grid BuildAndCacheGrid(string gridId)
        {
            logger.Debug("Building grid '{0}' and adding it to cache...", gridId);

            try
            {
                Grid grid = GridBuilder.BuildGrid(model[gridId], model.ScanParams, model.Style);
                grids.Add(gridId, grid);

                return grid;
            }
            catch (KeyNotFoundException)
            {
                string msg = String.Format("Keyboard '{0}' doesn't contain grid '{1}'!", Name, gridId);
                throw new Exception(msg);
            }
        }
    }
}
