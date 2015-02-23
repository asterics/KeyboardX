using NLog;
using Player.Conf;
using Player.Load.Element;
using Player.Model;
using Player.Model.Action;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Tags = Player.Conf.Tags.Button;

namespace Player.Load.Parse
{
    /// <summary>
    /// Parses a button model from XML file.<para />
    /// Can be used to parse several buttons one after another, but is not thread-safe!
    /// </summary>
    /// <remarks>
    /// How to add an action:
    /// <list type="bullet">
    ///     <item>
    ///     Add a method that parses the action and conforms to the delegate <c>Func&lt;ActionParameter&gt;</c>.
    ///     In case of error throw exception or return <c>NULL</c>.
    ///     </item>
    ///     <item>Register this method together with the action type keyword in <c>InitActionParsers()</c> to <c>actionParsers</c>.</item>
    /// </list>
    /// </remarks>
    class ButtonParser : BaseParser
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();


        private Button button;

        private StyleParser styleParser;

        private Dictionary<string, Func<ActionParameter>> actionParsers;


        public ButtonParser()
        {
            styleParser = new StyleParser();
            InitActionParsers();
            Reset();
        }


        public Button ParseButton(XmlReader rdr)
        {
            logger.Trace("ParseButton() called...");

            if (button != null)
                throw new InvalidOperationException("Don't parse several buttons simultaneously!");

            reader = rdr;

            string id = reader.GetAttribute(Tags.ButtonIdAttr);
            button = new Button(id);

            while (reader.Read())
            {
                logger.Trace(ElemLogMessage());

                if (IsElemNamed(Tags.PositionElem))
                    ReadPosition();
                else if (IsElemNamed(Tags.IconElem))
                    ReadIcon();
                else if (IsElemNamed(Tags.TextElem))
                    ReadText();
                else if (IsElemNamed(Tags.StyleElem))
                    button.Style = styleParser.ParseStyle(reader);
                else if (IsElemNamed(Tags.ActionElem))
                    ReadAction();
                else if (IsElemNamed(Tags.CloneElem))
                    button.CloneRef = reader.GetAttribute(Tags.CloneButtonAttr);
                else if (IsEndElemNamed(Tags.ButtonElem))
                {
                    logger.Trace("ParseButton() ends...");
                    break;
                }
                else
                    logger.Warn(ElemLogMessage());
            }

            Button btn = button;
            Reset();
            return btn;
        }

        protected override void Reset()
        {
            base.Reset();
            button = null;
        }

        private void ReadPosition()
        {
            int x = int.Parse(reader.GetAttribute(Tags.PositionXAttr));
            int y = int.Parse(reader.GetAttribute(Tags.PositionYAttr));

            int dimX, dimY;
            int.TryParse(reader.GetAttribute(Tags.PositionDimXAttr), out dimX);
            int.TryParse(reader.GetAttribute(Tags.PositionDimYAttr), out dimY);

            dimX = (dimX > 0 ? dimX : 1);
            dimY = (dimY > 0 ? dimY : 1);

            button.SetPosition(x, y, dimX, dimY);
        }

        private void ReadIcon()
        {
            Icon ic = new Icon();

            string mode = reader.GetAttribute(Tags.IconDisplayModeAttr);
            if (mode != null)
            {
                logger.Trace(AttrLogMessage(Tags.IconDisplayModeAttr, mode));
                ic.SetDisplayMode(mode);
            }

            reader.Read();
            logger.Trace(ElemLogMessage());
            string path = Path.Combine(Config.IconBaseDirectory, reader.ReadContentAsString());
            logger.Trace("icon path: " + path);
            ic.IconPath = path;

            button.Icon = ic;
        }

        private void ReadText()
        {
            logger.Trace("ReadText() called...");

            TextAlignment align = TextAlignment.Default;
            string al = reader.GetAttribute(Tags.TextAlignmentAttr);
            if (al != null)
            {
                logger.Trace(AttrLogMessage(Tags.TextAlignmentAttr, al));
                align = (TextAlignment)Enum.Parse(typeof(TextAlignment), al, true);
            }

            reader.Read();
            logger.Trace(ElemLogMessage());
            string text = reader.ReadContentAsString();
            button.SetText(text, align);
        }

        #region Actions

        private void InitActionParsers()
        {
            actionParsers = new Dictionary<string, Func<ActionParameter>>
            {
                { SwitchGridActionParameter.ActionTypeKeyword,      ReadSwitchGridAction },
                { LogActionParameter.ActionTypeKeyword,             ReadLogAction },
                { SelectActionParameter.ActionTypeKeyword,          ReadSelectAction },
                { ScannerActionParameter.ActionTypeKeyword,         ReadScannerAction },
                { TcpActionParameter.ActionTypeKeyword,             ReadTcpAction },
                { TimeActionParameter.ActionTypeKeyword,            ReadTimeAction },
                { TTSActionParameter.ActionTypeKeyword,             ReadTTSAction },
            };
        }

        private void ReadAction()
        {
            logger.Trace("ReadAction() called...");

            string type = reader.GetAttribute(Tags.ActionTypeAttr);

            if (String.IsNullOrEmpty(type))
                throw new LoaderException(CreatePosMessage("Type attribute for action may not be empty! {0}"));
            else
                logger.Trace(CreatePosMessage("Parsing action of type '{1}' {0}...", type));

            if (actionParsers.ContainsKey(type))
            {
                ActionParameter action = actionParsers[type]();
                if (action != null)
                    button.AddAction(action);
                else
                    throw new LoaderException(CreatePosMessage("Parsing action failed! {0}"));
            }
            else
                throw new LoaderException(CreatePosMessage("Action of type '{1}' is not implemented! {0}", type));
        }
        
        private ActionParameter ReadSwitchGridAction()
        {
            string gridId = reader.GetAttribute(Tags.ActionGridSwitchGridAttr);
            return new SwitchGridActionParameter(gridId);
        }

        private ActionParameter ReadLogAction()
        {
            reader.Read();
            string msg = reader.ReadContentAsString();
            return new LogActionParameter(msg);
        }

        private ActionParameter ReadSelectAction()
        {
            string buttonId = reader.GetAttribute(Tags.ActionSelectButtonAttr);
            return new SelectActionParameter(buttonId);
        }

        private ActionParameter ReadScannerAction()
        {
            try
            {
                string s = reader.GetAttribute(Tags.ActionScannerStartAttr);
                bool start = Boolean.Parse(s);
                return new ScannerActionParameter(start);
            }
            catch (Exception)
            {
                string msg = CreatePosMessage("Attribute '{1}' of type boolean is required for scanner action! {0}", Tags.ActionScannerStartAttr);
                throw new LoaderException(msg);
            }
        }

        private ActionParameter ReadTcpAction()
        {
            TcpDestination dst = null;
            string message = null;

            reader.Read();
            if (IsElemNamed(Tags.ActionTcpHostElem))
            {
                string host = reader.ReadElementContentAsString();
                int port = reader.ReadElementContentAsInt();
                dst = new TcpDestination(host, port);
                message = reader.ReadElementContentAsString();
            }
            else if (IsElemNamed(Tags.ActionTcpDestinationElem))
            {
                string dstId = reader.GetAttribute(Tags.ActionTcpRefAttr);
                dst = new TcpDestination(dstId);

                reader.Read();
                message = reader.ReadElementContentAsString();
            }
            else
                return null;

            return new TcpActionParameter(dst, message);
        }

        private ActionParameter ReadTimeAction()
        {
            reader.Read();
            TimeSpan time = TimeSpan.FromMilliseconds(reader.ReadContentAsInt());
            TimeActionParameter ta = new TimeActionParameter(time);
            return ta;
        }

        private ActionParameter ReadTTSAction()
        {
            reader.Read();
            string msg = reader.ReadContentAsString();
            return new TTSActionParameter(msg);
        }

        #endregion
    }
}
