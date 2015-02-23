using NLog;
using Player.Model;
using System;
using System.Collections.Generic;

namespace Player.Load.Element
{
    class Keyboard : KeyboardModel, KeyboardElement
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();


        public string Name { get; set; }

        private Dictionary<string, Grid> grids;

        public GridModel this[string gridId]
        {
            get
            {
                if (grids.ContainsKey(gridId))
                    return grids[gridId];
                else
                    throw new Exception(String.Format("Model for keyboard '{0}' doesn't contain grid '{1}'!", Name, gridId));
            }
        }

        private string _defaultGridId;
        public string DefaultGridId
        {
            get { return _defaultGridId; }

            set
            {
                if (grids.ContainsKey(value))
                    _defaultGridId = value;
                else
                    throw new Exception(String.Format("Can't set '{0}' to default grid, cause keyboard model doesn't contain this grid!", value));
            }
        }

        public GridModel DefaultGrid
        {
            get { return grids[DefaultGridId]; }
        }

        /// <summary>Helper property for parser. For saving id of first grid, if no default is defined it will be used.</summary>
        public string FirstGridId { get; set; }

        public ScannerParameter ScanParams { get; set; }

        public GeneralStyle Style { get; set; }


        public Keyboard()
        {
            grids = new Dictionary<string, Grid>();
        }


        public void AddGrid(Grid g)
        {
            if (grids.ContainsKey(g.Id))
                throw new InvalidOperationException(String.Format("There is already a grid with the id of '{0}'!", g.Id));

            g.CloneButtons();

            grids.Add(g.Id, g);
        }

        public Grid GetGridModel(string id)
        {
            if (id == null)
                return null;

            if (grids.ContainsKey(id))
                return grids[id];
            else
                return null;
        }

        public void Validate()
        {
            logger.Trace("Validating keyboard '{0}'...", Name);

            if (grids.Count == 0)
                throw new LoaderException("A keyboard must contain at least one grid!");

            if (String.IsNullOrEmpty(DefaultGridId))
                throw new LoaderException("One grid of keyboard must be marked as default!");

            foreach (Grid g in grids.Values)
                g.Validate();
        }
    }
}
