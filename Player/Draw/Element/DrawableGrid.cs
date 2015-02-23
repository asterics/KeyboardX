using NLog;
using Player.Core.Status;
using Player.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace Player.Draw.Element
{
    /// <summary>
    /// Wraps a <see cref="GridModel"/> instance and extends it with stuff needed for drawing.
    /// </summary>
    /// TODOs:
    ///  TODO 2: provide a GetButton(Point p) function for OnClick and such
    class DrawableGrid : IEnumerable<DrawableButton>, IDisposable
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();


        protected GridModel model;

        public int Cols
        {
            get { return model.Cols; }
        }

        public int Rows
        {
            get { return model.Rows; }
        }

        protected DrawableButton[,] buttonGrid;

        private DrawableButton this[int x, int y]
        {
            get { return buttonGrid[x, y]; }
        }

        protected Dictionary<string, DrawableButton> buttonDict;

        private DrawableButton this[string buttonId]
        {
            get { return buttonDict[buttonId]; }
        }

        private Brush _backBrush;
        public Brush BackBrush
        {
            get
            {
                if (_backBrush == null)
                    _backBrush = CreateBrush(model.Style.GridBackColor);

                return _backBrush;
            }
        }

        private Brush _borderBrush;
        public Brush BorderBrush
        {
            get
            {
                if (_borderBrush == null)
                    _borderBrush = CreateBrush(model.Style.GridBorderColor);

                return _borderBrush;
            }
        }

        private Brush _selectBrush;
        public Brush SelectBrush
        {
            get
            {
                if (_selectBrush == null)
                    _selectBrush = CreateBrush(model.Style.SelectColor);

                return _selectBrush;
            }
        }

        private Brush _mouseBrush;
        public Brush MouseBrush
        {
            get
            {
                if (_mouseBrush == null)
                    _mouseBrush = CreateBrush(model.Style.MouseColor);

                return _mouseBrush;
            }
        }


        public DrawableGrid(GridModel grid, GridStatus status)
        {
            model = grid;
            InitButtons(status);
        }


        public void Dispose()
        {
            foreach (var btn in buttonDict.Values)
                btn.Dispose();

            if (_backBrush != null) _backBrush.Dispose();
            if (_borderBrush != null) _borderBrush.Dispose();
            if (_selectBrush != null) _selectBrush.Dispose();
            if (_mouseBrush != null) _mouseBrush.Dispose();
        }

        // explicitly define interface implementation
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<DrawableButton> GetEnumerator()
        {
            return buttonDict.Values.GetEnumerator();
        }

        protected virtual void InitButtons(GridStatus status)
        {
            buttonDict = new Dictionary<string, DrawableButton>();
            foreach (var btn in model)
                buttonDict[btn.Id] = new DrawableButton(btn, status[btn.Id]);

            buttonGrid = new DrawableButton[Cols, Rows];
            for (int x = 0; x < Cols; x++)
                for (int y = 0; y < Rows; y++)
                    if (model[x, y] != null)
                        buttonGrid[x, y] = buttonDict[model[x, y].Id];  // don't create objects again
        }

        private Brush CreateBrush(string color)
        {
            Color c = ColorTranslator.FromHtml(color);
            return new SolidBrush(c);
        }
    }
}
