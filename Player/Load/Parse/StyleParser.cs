using NLog;
using Player.Load.Element;
using System;
using System.Xml;
using Tags = Player.Conf.Tags.Style;

namespace Player.Load.Parse
{
    /// <summary>
    /// Parses style element from keyboard file.<para />
    /// Can be used to parse several style elements one after another, but is not thread-safe!
    /// </summary>
    /// TODO 3: [conf] Load.Parse.StyleParser and Load.Element.Style belong to Conf namespace probably
    class StyleParser : BaseParser
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();


        public StyleParser()
        {
            Reset();
        }


        public Style ParseStyle(XmlReader rdr)
        {
            logger.Trace("ParseStyle() called...");

            reader = rdr;
            Style style = new Style();

            while (reader.Read())
            {
                logger.Trace(ElemLogMessage());

                if (IsElemNamed(Tags.DrawerTypeElem))
                    style.DrawerType = ReadStringElem(Tags.DrawerTypeElem);
                else if (IsElemNamed(Tags.GridBackColorElem))
                    style.GridBackColor = ReadStringElem(Tags.GridBackColorElem);
                else if (IsElemNamed(Tags.GridBorderColorElem))
                    style.GridBorderColor = ReadStringElem(Tags.GridBorderColorElem);
                else if (IsElemNamed(Tags.SelectColorElem))
                    style.SelectColor = ReadStringElem(Tags.SelectColorElem);
                else if (IsElemNamed(Tags.MouseColorElem))
                    style.MouseColor = ReadStringElem(Tags.MouseColorElem);
                else if (IsElemNamed(Tags.BorderWidthElem))
                    style.BorderWidth = ReadIntElem(Tags.BorderWidthElem);
                else if (IsElemNamed(Tags.MarginWidthElem))
                    style.MarginWidth = ReadIntElem(Tags.MarginWidthElem);
                else if (IsElemNamed(Tags.GapWidthElem))
                    style.GapWidth = ReadIntElem(Tags.GapWidthElem);
                else if (IsElemNamed(Tags.ButtonBackColorElem))
                    style.ButtonBackColor = ReadStringElem(Tags.ButtonBackColorElem);
                else if (IsElemNamed(Tags.ButtonBorderColorElem))
                    style.ButtonBorderColor = ReadStringElem(Tags.ButtonBorderColorElem);
                else if (IsElemNamed(Tags.ButtonFontColorElem))
                    style.ButtonFontColor = ReadStringElem(Tags.ButtonFontColorElem);
                else if (IsElemNamed(Tags.ButtonFontSizeElem))
                    style.ButtonFontSize = ReadIntElem(Tags.ButtonFontSizeElem);
                else if (IsEndElemNamed(Tags.StyleElem))
                {
                    logger.Trace("ParseStyle() ends...");
                    break;
                }
                else
                    logger.Warn(ElemLogMessage());
            }

            Reset();
            return style;
        }
    }
}
