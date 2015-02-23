using NLog;
using Player.Conf;
using Player.Load.Element;
using Player.Util;
using System;
using System.IO;
using System.Xml;

namespace Player.Load.Parse
{
    /// <summary>
    /// Parses a keyboard model from XML file.<para />
    /// Should be only used to parse one keyboard and is not thread-safe!
    /// </summary>
    /// TODOs:
    ///  TODO 1: check if it could be used to read more than one keyboard files
    class KeyboardParser : BaseParser
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private StyleParser styleParser;
        private ButtonParser buttonParser;


        public KeyboardParser()
        {
            styleParser = new StyleParser();
            buttonParser = new ButtonParser();
            Reset();
        }


        public Keyboard ParseKeyboard(Stream keyboardStream)
        {
            try
            {
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.IgnoreComments = true;
                settings.IgnoreWhitespace = true;

                using (reader = XmlReader.Create(keyboardStream, settings))
                {
                    return ReadDocument();
                }
            }
            catch (LoaderException) { throw; }
            catch (NotImplementedException) { throw; }
            catch (Exception e)
            {
                string msg = "Unrecognized exception occurred while parsing keyboard!";
                throw new LoaderException(msg, e);
            }
            finally
            {
                Reset();
            }
        }

        private Keyboard ReadDocument()
        {
            try
            {
                while (reader.Read())  // read elements until we reach an opening keyboard tag
                {
                    logger.Trace(ElemLogMessage());

                    if (IsElemNamed(Tags.Keyboard.KeyboardElem))
                        return ReadKeyboard();
                    else if (reader.NodeType == XmlNodeType.XmlDeclaration)
                        { /* ignore */ }
                    else
                        logger.Warn(ElemLogMessage());
                }
            }
            catch (XmlException e)
            {
                if (e.InnerException != null && e.InnerException is XmlException)  // comes from XmlUtil -> transform to LoaderException
                    throw new LoaderException(e.Message, e.InnerException);
                else
                {
                    string msg = "Something is wrong with the XML structure of the keyboard!";
                    throw new LoaderException(msg, e);
                }
            }
            
            throw new LoaderException("No keyboard model could be found in given file!");
        }

        private Keyboard ReadKeyboard()
        {
            logger.Trace("ReadKeyboard() called...");

            Keyboard kbd = new Keyboard();

            while (reader.Read())
            {
                logger.Trace(ElemLogMessage());

                if (IsElemNamed(Tags.Grid.GridElem))
                    AddGrid(kbd);
                else if (IsElemNamed(Tags.Keyboard.DefaultElem))
                    ReadKeyboardDefault(kbd);
                else if (IsEndElemNamed(Tags.Keyboard.KeyboardElem))
                {
                    logger.Trace("ReadKeyboard() ends...");
                    break;
                }
                else
                    logger.Warn(ElemLogMessage());
            }

            if (String.IsNullOrEmpty(kbd.DefaultGridId))
                kbd.DefaultGridId = kbd.FirstGridId;

            return kbd;
        }

        private void ReadKeyboardDefault(Keyboard kbd)
        {
            logger.Trace("ReadDefault() called...");

            while (reader.Read())
            {
                if (IsElemNamed(Tags.Scanner.ScannerElem))
                    kbd.ScanParams = ReadScanner();
                else if (IsElemNamed(Tags.Style.StyleElem))
                    kbd.Style = styleParser.ParseStyle(reader);
                else if (IsEndElemNamed(Tags.Keyboard.DefaultElem))
                {
                    logger.Trace("ReadDefault() ends...");
                    break;
                }
                else
                    logger.Warn(ElemLogMessage());
            }
        }

        private void AddGrid(Keyboard kbd)
        {
            Grid grid = ReadGrid();
            kbd.AddGrid(grid);

            if (kbd.FirstGridId == null)
                kbd.FirstGridId = grid.Id;

            if (grid.Default)
            {
                if (!String.IsNullOrEmpty(kbd.DefaultGridId))
                    logger.Warn("Only one grid should be marked as default! Not '{0}' and '{1}'!", kbd.DefaultGridId, grid.Id);

                kbd.DefaultGridId = grid.Id;
            }
        }

        private Grid ReadGrid()
        {
            logger.Trace("ReadGrid() called...");

            string id = reader.GetAttribute(Tags.Grid.GridIdAttr);
            Grid grid = new Grid(id);

            grid.Default = XmlUtil.ReadAttributeAsOptionalBool(reader, Tags.Grid.GridDefaultAttr);

            while (reader.Read())
            {
                logger.Trace(ElemLogMessage());
              
                if (IsElemNamed(Tags.Button.ButtonElem))
                {
                    Button b = buttonParser.ParseButton(reader);
                    grid.AddButton(b);
                }
                else if (IsElemNamed(Tags.Grid.DimensionElem))
                {
                    int cols = int.Parse(reader.GetAttribute(Tags.Grid.DimensionColsAttr));
                    int rows = int.Parse(reader.GetAttribute(Tags.Grid.DimensionRowsAttr));
                    grid.SetDimension(cols, rows);
                }
                else if (IsElemNamed(Tags.Scanner.ScannerElem))
                    grid.ScanParams = ReadScanner();
                else if (IsElemNamed(Tags.Style.StyleElem))
                    grid.Style = styleParser.ParseStyle(reader);
                else if (IsEndElemNamed(Tags.Grid.GridElem))
                {
                    logger.Trace("ReadGrid() ends...");
                    break;
                }
                else
                    logger.Warn(ElemLogMessage());
            }

            return grid;
        }

        private ScannerParam ReadScanner()
        {
            logger.Trace("ReadScanner() called...");

            ScannerParam scp = new ScannerParam();

            while (reader.Read())
            {
                logger.Trace(ElemLogMessage());

                if (IsElemNamed(Tags.Scanner.ScannerActiveElem))
                    scp.ScannerActive = ReadBoolElem(Tags.Scanner.ScannerActiveElem);
                else if (IsElemNamed(Tags.Scanner.ScannerTypeElem))
                    scp.ScannerType = ReadStringElem(Tags.Scanner.ScannerTypeElem);
                else if (IsElemNamed(Tags.Scanner.InitialScanDelayElem))
                    scp.InitialScanDelay = ReadIntElem(Tags.Scanner.InitialScanDelayElem);
                else if (IsElemNamed(Tags.Scanner.InputAcceptanceTimeElem))
                    scp.InputAcceptanceTime = ReadIntElem(Tags.Scanner.InputAcceptanceTimeElem);
                else if (IsElemNamed(Tags.Scanner.PostAcceptanceDelayElem))
                    scp.PostAcceptanceDelay = ReadIntElem(Tags.Scanner.PostAcceptanceDelayElem);
                else if (IsElemNamed(Tags.Scanner.PostInputAcceptanceTimeElem))
                    scp.PostInputAcceptanceTime = ReadIntElem(Tags.Scanner.PostInputAcceptanceTimeElem);
                else if (IsElemNamed(Tags.Scanner.ScanTimeElem))
                    scp.ScanTime = ReadIntElem(Tags.Scanner.ScanTimeElem);
                else if (IsElemNamed(Tags.Scanner.StartLeftElem))
                    scp.StartLeft = ReadBoolElem(Tags.Scanner.StartLeftElem);
                else if (IsElemNamed(Tags.Scanner.StartTopElem))
                    scp.StartTop = ReadBoolElem(Tags.Scanner.StartTopElem);
                else if (IsElemNamed(Tags.Scanner.MoveHorizontalElem))
                    scp.MoveHorizontal = ReadBoolElem(Tags.Scanner.MoveHorizontalElem);
                else if (IsElemNamed(Tags.Scanner.LocalCycleLimitElem))
                    scp.LocalCycleLimit = ReadIntElem(Tags.Scanner.LocalCycleLimitElem);
                else if (IsEndElemNamed(Tags.Scanner.ScannerElem))
                {
                    logger.Trace("ReadScanner() ends...");
                    break;
                }
                else
                    logger.Warn(ElemLogMessage());
            }

            return scp;
        }
    }
}
