using NLog;
using Player.Model;
using Player.Model.Action;
using System;
using System.Collections.Generic;
using System.IO;

namespace Player.Load.Element
{
    class Button : ButtonModel, KeyboardElement
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();


        public string Id { get; private set; }

        public ButtonPosition Position { get; protected set; }

        /// <summary>Clone reference, the id of the button of which this button should be cloned from.</summary>
        public string CloneRef { get; set; }

        public ButtonIcon Icon { get; set; }

        public ButtonText Text { get; protected set; }

        public GeneralStyle Style { get; set; }

        protected List<ActionParameter> actions;

        public ActionParameter[] ActionParams
        {
            get { return actions.ToArray(); }
        }


        public Button(string id)
        {
            Id = id;
            actions = new List<ActionParameter>();
        }


        public void SetPosition(int x, int y)
        {
            SetPosition(x, y, 1, 1);
        }

        public void SetPosition(int x, int y, int dimX, int dimY)
        {
            if (Position != null)
                throw new InvalidOperationException("Position is allowed to be set only once!");

            Position = new PositionImpl(x, y, dimX, dimY);
        }

        public void SetText(string text)
        {
            SetText(text, TextAlignment.Default);
        }

        public void SetText(string text, TextAlignment align)
        {
            Text = new TextImpl(text, align);
        }

        public void AddAction(ActionParameter action)
        {
            actions.Add(action);
        }

        /// <summary>Clones everything from <paramref name="btn"/> except id and position.</summary>
        public void CloneFrom(Button btn)
        {
            // TODO 3: cloning of button is not complete
            logger.Trace("Cloning button '{0}' from '{1}'...", Id, btn.Id);
            Icon = btn.Icon;
            Text = btn.Text;
            Style = btn.Style;
            actions = btn.actions;
        }

        public void Validate()
        {
            logger.Trace("Validating button '{0}'...", Id);

            CheckForTextOrIcon();
            CheckIconPath();
        }

        protected void CheckForTextOrIcon()
        {
            if ((Text == null || Text.Content == null) && (Icon == null || Icon.IconPath == null))
            {
                string msg = String.Format("Either text or icon has to be set for button '{0}'!", Id);
                throw new LoaderException(msg);
            }
        }

        protected void CheckIconPath()
        {
            if (Icon != null && Icon.IconPath != null)
            {
                string path = Icon.IconPath;

                if (!File.Exists(path))
                {
                    string msg = String.Format("Icon for button '{0}' couldn't be found! (path: \"{1}\")", Id, path);
                    throw new LoaderException(msg);
                }
            }
        }


        protected struct PositionImpl : ButtonPosition
        {
            public int X { get; private set; }
            public int Y { get; private set; }
            public int DimX { get; private set; }
            public int DimY { get; private set; }


            public PositionImpl(int x, int y, int dimX, int dimY)
                : this()
            {
                X = x; Y = y; DimX = dimX; DimY = dimY;
            }
        }

        protected struct TextImpl : ButtonText
        {
            public string Content { get; private set; }
            public TextAlignment Alignment { get; private set; }


            public TextImpl(string text, TextAlignment align)
                : this()
            {
                Content = text;
                Alignment = align;
            }
        }
    }
}
