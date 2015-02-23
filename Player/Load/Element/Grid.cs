using NLog;
using Player.Core;
using Player.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Player.Load.Element
{
    class Grid : GridModel, KeyboardElement
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();


        public string Id { get; private set; }

        public bool Default { get; set; }

        public int Cols { get; private set; }

        public int Rows { get; private set; }

        private Button[,] buttonGrid;

        public ButtonModel this[int x, int y]
        {
            get { return buttonGrid[x, y]; }
        }

        private Dictionary<string, Button> buttonDict;

        public ButtonModel this[string buttonId]
        {
            get { return buttonDict[buttonId]; }
        }

        public ScannerParameter ScanParams { get; set; }

        public GeneralStyle Style { get; set; }


        public Grid(string id)
        {
            Id = id;
            buttonDict = new Dictionary<string, Button>();
        }


        // explicitly define interface implementation
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<ButtonModel> GetEnumerator()
        {
            return buttonDict.Values.GetEnumerator();
        }

        public ButtonGroup GetButtonsForCol(int x)
        {
            CheckColArgument(x);

            ButtonGroup group = new ButtonGroup();
            for (int y = 0; y < Rows; y++)
                if (buttonGrid[x, y] != null)
                    group.Add(buttonGrid[x, y].Id);

            group.Seal();
            return group;
        }

        public ButtonGroup GetButtonsForRow(int y)
        {
            CheckRowArgument(y);

            ButtonGroup group = new ButtonGroup();
            for (int x = 0; x < Cols; x++)
                if (buttonGrid[x, y] != null)
                    group.Add(buttonGrid[x, y].Id);

            group.Seal();
            return group;
        }

        public ButtonGroup GetButtonsForArea(int index)
        {
            throw new NotImplementedException();
        }

        public void AddButton(Button btn)
        {
            if (buttonGrid == null)
                throw new InvalidOperationException("Method SetDimension() has to be called before adding a button!");

            if (btn == null)
                throw new ArgumentNullException("Parameter 'btn' may not be null!");

            ButtonPosition pos = btn.Position;
            if (pos.X < 0 || pos.X >= Cols || (pos.X + pos.DimX > Cols))
                throw new ArgumentOutOfRangeException(String.Format("Position in x-axis for button '{0}' is out of range!", btn.Id));

            if (pos.Y < 0 || pos.Y >= Rows || (pos.Y + pos.DimY > Rows))
                throw new ArgumentOutOfRangeException(String.Format("Position in y-axis for button '{0}' is out of range!", btn.Id));

            if (buttonDict.ContainsKey(btn.Id))
                throw new ArgumentException(String.Format("Button with id '{0}' was already added!", btn.Id));

            for (int i = pos.X; i < pos.X + pos.DimX; i++)
                for (int j = pos.Y; j < pos.Y + pos.DimY; j++)
                    buttonGrid[i, j] = btn;

            buttonDict.Add(btn.Id, btn);
        }

        public void SetDimension(int cols, int rows)
        {
            if (cols < 1)
                throw new ArgumentOutOfRangeException("Argument 'cols' is not valid!");

            if (rows < 1)
                throw new ArgumentOutOfRangeException("Argument 'rows' is not valid!");

            Cols = cols;
            Rows = rows;
            buttonGrid = new Button[cols, rows];
        }

        /// <summary>Clones buttons witch are referencing to another in model.</summary>
        public void CloneButtons()
        {
            try
            {
                foreach (var btn in buttonDict.Values)
                    if (!String.IsNullOrEmpty(btn.CloneRef))
                        btn.CloneFrom(buttonDict[btn.CloneRef]);
            }
            catch (Exception e)
            {
                throw new LoaderException("Cloning buttons failed!", e);
            }
        }

        public void Validate()
        {
            logger.Trace("Validating grid '{0}'...", Id);

            if (buttonDict.Count == 0)
                throw new LoaderException("A grid must contain at least one button!");

            foreach (var btn in buttonDict.Values)
                btn.Validate();
        }

        private void CheckColArgument(int x)
        {
            if (x < 0 || x >= Cols)
                throw new ArgumentOutOfRangeException("x", "Argument 'x' is out of range!");
        }

        private void CheckRowArgument(int y)
        {
            if (y < 0 || y >= Rows)
                throw new ArgumentOutOfRangeException("y", "Argument 'y' is out of range!");
        }
    }
}
