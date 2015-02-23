using NLog;
using Player.Conf;
using Player.Core.Gui;
using Player.Core.Scan;
using Player.Core.Status;
using Player.Model;
using System;
using System.Collections.Generic;

namespace Player.Core.Element
{
    /// <summary>
    /// Represents a grid element. Is a wrapper around <see cref="GridModel"/>.
    /// </summary>
    /// <remarks>
    /// A <see cref="Grid"/> (as well as a <see cref="GridStatus"/>) object is created when a new grid is loaded. This happens at startup or on 
    /// grid switch. When grid is switched, we should keep a reference to the old grid and it's status. When we switch back to it and it's still 
    /// in memory, this could be pretty damn fast.
    /// </remarks>
    class Grid
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();


        private GridModel model;

        public string Id
        {
            get { return model.Id; }
        }

        public int Cols
        {
            get { return model.Cols; }
        }

        public int Rows
        {
            get { return model.Rows; }
        }

        private Button[,] buttonGrid;

        // TODO 1: if positional indexer is indeed not needed, remove it (see also #InitButtons()!)
        private Button this[int x, int y]
        {
            get
            {
                try { return buttonGrid[x, y]; }
                catch (IndexOutOfRangeException)
                {
                    string msg = String.Format("Grid '{0}' has (only) a dimension of [{1}, {2}]!", Id, Cols, Rows);
                    throw new Exception(msg);
                }
            }
        }

        private Dictionary<string, Button> buttonDict;

        public Button this[string buttonId]
        {
            get
            {
                try { return buttonDict[buttonId]; }
                catch (KeyNotFoundException)
                {
                    string msg = String.Format("Grid '{0}' doesn't contain button '{1}'!", Id, buttonId);
                    throw new Exception(msg);
                }
            }
        }

        public GridStatusImpl Status { get; private set; }

        public Drawer Drawer { get; private set; }

        public Scanner Scanner { get; private set; }


        /// <summary>Should only be used by <see cref="GridBuilder"/>!</summary>
        public Grid(GridModel model, GridStatusImpl status, Drawer drawer, Scanner scanner)
        {
            this.model = model;
            Status = status;
            Drawer = drawer;
            Scanner = scanner;
            InitButtons();
        }


        private void InitButtons()
        {
            buttonDict = new Dictionary<string, Button>();
            foreach (var btn in model)
                buttonDict[btn.Id] = new Button(btn);

            buttonGrid = new Button[Cols, Rows];
            for (int x = 0; x < Cols; x++)
                for (int y = 0; y < Rows; y++)
                    if (model[x, y] != null)
                        buttonGrid[x, y] = buttonDict[model[x, y].Id];  // don't create objects again
        }
    }
}
