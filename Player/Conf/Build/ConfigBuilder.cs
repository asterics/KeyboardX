using NLog;
using Player.Load.Element;
using Player.Load.Parse;
using Player.Model;
using Player.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Player.Conf.Build
{
    /// <summary>
    /// Builds config by merging hard coded values and a XML config file.
    /// </summary>
    class ConfigBuilder
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();


        public GlobalConfigImpl GlobalConfig { get; private set; }

        public ScannerConfigImpl ScannerConfig { get; private set; }

        public GeneralStyle StyleConfig { get; private set; }

        private XmlReader reader;


        public ConfigBuilder()
        {
            GlobalConfig = new GlobalConfigImpl();
            ScannerConfig = new ScannerConfigImpl();
            StyleConfig = GetDefaultStyleConfig();
        }


        /// <summary>
        /// Reads values from config file and merge them with hard coded values.
        /// </summary>
        public void BuildConfig()
        {
            logger.Trace("BuildConfig()...");

            try
            {
                string configFileName = Properties.Settings.Default.ConfigFileName;

                if (!File.Exists(configFileName))
                {
                    string msg = String.Format("Given config file couldn't be found! ({0})", 
                            Path.Combine(Directory.GetCurrentDirectory(), configFileName));
                    throw new ConfigException(msg);
                }

                using (Stream configStream = OpenConfig(configFileName))
                {
                    ParseConfig(configStream);
                }
            }
            catch (ConfigException) { throw; }
            catch (NotImplementedException) { throw; }
            catch (Exception e)
            {
                string msg = "Unrecognized exception occurred while loading config!";
                throw new ConfigException(msg, e);
            }
        }

        private Stream OpenConfig(string path)
        {
            FileStream fs = null;

            try
            {
                fs = new FileStream(path, FileMode.Open);
            }
            catch (Exception e)
            {
                string msg = "Opening config file failed!";
                throw new ConfigException(msg, e);
            }

            return fs;
        }

        private void ParseConfig(Stream configStream)
        {
            try
            {
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.IgnoreComments = true;
                settings.IgnoreWhitespace = true;

                using (reader = XmlReader.Create(configStream, settings))
                {
                    ReadDocument();
                }
            }
            catch (ConfigException) { throw; }
            catch (NotImplementedException) { throw; }
            catch (Exception e)
            {
                string msg = "Unrecognized exception occurred while parsing config!";
                throw new ConfigException(msg, e);
            }
        }

        private void ReadDocument()
        {
            try
            {
                while (reader.Read())  // read elements until we reach an opening config tag
                {
                    logger.Trace(ElemLogMsg());

                    if (IsElemNamed(Tags.Config.ConfigElem))
                        ReadConfig();
                    else if (reader.NodeType == XmlNodeType.XmlDeclaration)
                        { /* ignore */ }
                    else
                        logger.Warn(ElemLogMsg());
                }
                return;
            }
            catch (XmlException e)
            {
                if (e.InnerException != null && e.InnerException is XmlException)  // comes from XmlUtil -> transform to ConfigException
                    throw new ConfigException(e.Message, e.InnerException);
                else
                {
                    string msg = "Something is wrong with the XML structure of config!";
                    throw new ConfigException(msg, e);
                }
            }

            throw new ConfigException("No config section could be found in config file!");
        }

        private void ReadConfig()
        {
            logger.Trace("ReadConfig() called...");

            while (reader.Read())
            {
                logger.Trace(ElemLogMsg());

                if (IsElemNamed(Tags.Config.GlobalElem))
                    ReadGlobal();
                else if (IsElemNamed(Tags.Scanner.ScannerElem))
                    ReadScanner();
                else if (IsElemNamed(Tags.Style.StyleElem))
                    ReadStyle();
                else if (IsEndElemNamed(Tags.Config.ConfigElem))
                {
                    logger.Trace("ReadConfig() ends...");
                    break;
                }
                else
                    logger.Warn(ElemLogMsg());
            }
        }

        private void ReadGlobal()
        {
            logger.Trace("ReadGlobal() called...");

            while (reader.Read())
            {
                logger.Trace(ElemLogMsg());

                if (IsElemNamed(Tags.Config.SchemaFileNameElem))
                    GlobalConfig.SchemaFileName = ReadString(Tags.Config.SchemaFileNameElem);
                else if (IsElemNamed(Tags.Config.SchemaValidationElem))
                    GlobalConfig.SchemaValidation = ReadBool(Tags.Config.SchemaValidationElem);
                else if (IsElemNamed(Tags.Config.ClickTriggerActiveElem))
                    GlobalConfig.ClickTriggerActive = ReadBool(Tags.Config.ClickTriggerActiveElem);
                else if (IsElemNamed(Tags.Config.ShowMouseChangesElem))
                    GlobalConfig.ShowMouseChanges = ReadBool(Tags.Config.ShowMouseChangesElem);
                else if (IsElemNamed(Tags.Config.ButtonPressSynchronous))
                    GlobalConfig.ButtonPressSynchronous = ReadBool(Tags.Config.ButtonPressSynchronous);
                else if (IsElemNamed(Tags.Config.NetElem))
                    ReadNetElem();
                else if (IsEndElemNamed(Tags.Config.GlobalElem))
                {
                    logger.Trace("ReadGlobal() ends...");
                    break;
                }
                else
                    logger.Warn(ElemLogMsg());
            }
        }

        private void ReadNetElem()
        {
            logger.Trace("ReadNetElem() called...");

            List<TcpDestinationModel> tcpDests = null;

            while (reader.Read())
            {
                logger.Trace(ElemLogMsg());

                if (IsElemNamed(Tags.Config.TcpDestinationElem))
                {
                    if (tcpDests == null)
                        tcpDests = new List<TcpDestinationModel>();

                    tcpDests.Add(ReadTcpDest());
                }
                else if (IsElemNamed(Tags.Config.TcpSinkElem))
                    ReadTcpSink();
                else if (IsEndElemNamed(Tags.Config.NetElem))
                {
                    logger.Trace("ReadNetElem() ends...");
                    break;
                }
                else
                    logger.Warn(ElemLogMsg());
            }

            if (tcpDests != null)
                GlobalConfig.TcpDests = tcpDests;
        }

        private TcpDestinationModel ReadTcpDest()
        {
            TcpDest dest = new TcpDest();

            dest.Id = reader.GetAttribute(Tags.Config.TcpDestinationIdAttr);
            reader.Read();
            dest.Host = ReadString(Tags.Config.TcpDestinationHostElem);
            reader.Read();
            dest.Port = ReadInt(Tags.Config.TcpDestinationPortElem);
            reader.Read();

            return dest;
        }

        private struct TcpDest : TcpDestinationModel
        {
            public string Id { get; set; }
            public string Host { get; set; }
            public int Port { get; set; }
        }

        private void ReadTcpSink()
        {
            if (XmlUtil.ReadAttributeAsBool(reader, Tags.Config.TcpSinkActiveAttr))
            {
                GlobalConfig.TcpSinkActive = true;
                GlobalConfig.TcpSinkPort = XmlUtil.ReadAttributeAsInt(reader, Tags.Config.TcpSinkPortAttr);
            }
            else
                GlobalConfig.TcpSinkActive = false;
        }

        private void ReadScanner()
        {
            logger.Trace("ReadScanner() called...");

            while (reader.Read())
            {
                logger.Trace(ElemLogMsg());

                if (IsElemNamed(Tags.Scanner.ScannerTypeElem))
                    ScannerConfig.ScannerType = ReadString(Tags.Scanner.ScannerTypeElem);
                else if (IsElemNamed(Tags.Scanner.ScannerActiveElem))
                    ScannerConfig.ScannerActive = ReadBool(Tags.Scanner.ScannerActiveElem);
                else if (IsElemNamed(Tags.Scanner.InitialScanDelayElem))
                    ScannerConfig.InitialScanDelay = ReadInt(Tags.Scanner.InitialScanDelayElem);
                else if (IsElemNamed(Tags.Scanner.InputAcceptanceTimeElem))
                    ScannerConfig.InputAcceptanceTime = ReadInt(Tags.Scanner.InputAcceptanceTimeElem);
                else if (IsElemNamed(Tags.Scanner.PostAcceptanceDelayElem))
                    ScannerConfig.PostAcceptanceDelay = ReadInt(Tags.Scanner.PostAcceptanceDelayElem);
                else if (IsElemNamed(Tags.Scanner.PostInputAcceptanceTimeElem))
                    ScannerConfig.PostInputAcceptanceTime = ReadInt(Tags.Scanner.PostInputAcceptanceTimeElem);
                else if (IsElemNamed(Tags.Scanner.RepeatAcceptanceDelayElem))
                    ScannerConfig.RepeatAcceptanceDelay = ReadInt(Tags.Scanner.RepeatAcceptanceDelayElem);
                else if (IsElemNamed(Tags.Scanner.RepeatTimeElem))
                    ScannerConfig.RepeatTime = ReadInt(Tags.Scanner.RepeatTimeElem);
                else if (IsElemNamed(Tags.Scanner.ScanTimeElem))
                    ScannerConfig.ScanTime = ReadInt(Tags.Scanner.ScanTimeElem);
                else if (IsElemNamed(Tags.Scanner.StartLeftElem))
                    ScannerConfig.StartLeft = ReadBool(Tags.Scanner.StartLeftElem);
                else if (IsElemNamed(Tags.Scanner.StartTopElem))
                    ScannerConfig.StartTop = ReadBool(Tags.Scanner.StartTopElem);
                else if (IsElemNamed(Tags.Scanner.MoveHorizontalElem))
                    ScannerConfig.MoveHorizontal = ReadBool(Tags.Scanner.MoveHorizontalElem);
                else if (IsElemNamed(Tags.Scanner.LocalCycleLimitElem))
                    ScannerConfig.LocalCycleLimit = ReadInt(Tags.Scanner.LocalCycleLimitElem);
                else if (IsEndElemNamed(Tags.Scanner.ScannerElem))
                {
                    logger.Trace("ReadScan() ends...");
                    break;
                }
                else
                    logger.Warn(ElemLogMsg());
            }
        }

        private GeneralStyle GetDefaultStyleConfig()
        {
            Style style = new Style();  // for now borrow class from Load.Element namespace

            style.DrawerType = "border";
            style.GridBackColor = "LightGray";
            style.GridBorderColor = "LightSlateGray";
            style.SelectColor = "green";
            style.MouseColor = "MidnightBlue";
            style.BorderWidth = 3;
            style.MarginWidth = 3;
            style.GapWidth = 4;
            style.ButtonBackColor = "Seashell";
            style.ButtonBorderColor = "TODO";
            style.ButtonFontColor = "black";
            style.ButtonFontSize = -1;

            return style;
        }

        private void ReadStyle()
        {
            StyleParser styleParser = new StyleParser();
            Style style = styleParser.ParseStyle(reader);
            style.InheritFrom(StyleConfig);
            StyleConfig = style;

            if (!StyleConfig.IsComplete())
                throw new ConfigException("StyleConfig has to be complete!");
        }

        // used a lot and therefore shortened wrapper methods
        #region UtilWrapper

        private bool IsElemNamed(string name)
        {
            return XmlUtil.IsElementNamed(reader, name);
        }

        private bool IsEndElemNamed(string name)
        {
            return XmlUtil.IsEndElementNamed(reader, name);
        }

        private bool ReadBool(string elemName)
        {
            return XmlUtil.ReadElementAsBool(reader, elemName);
        }

        private int ReadInt(string elemName)
        {
            return XmlUtil.ReadElementAsInt(reader, elemName);
        }

        private string ReadString(string elemName)
        {
            return XmlUtil.ReadElementAsString(reader, elemName);
        }

        private string ElemLogMsg()
        {
            return XmlUtil.CreateElementLogMessage(reader);
        }

        #endregion
    }
}
