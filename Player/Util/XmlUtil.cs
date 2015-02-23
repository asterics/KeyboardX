using System;
using System.Xml;

namespace Player.Util
{
    /// <summary>
    /// Provides utility methods for XML parsing and stuff.
    /// </summary>
    static class XmlUtil
    {
        public static string CreateAttributeLogMessage(string attrName, string attrValue)
        {
            return String.Format("read attribute '{0}' with content '{1}'", attrName, attrValue);
        }

        public static string CreateElementLogMessage(XmlReader reader)
        {
            string content = (reader.NodeType == XmlNodeType.Text ? reader.Value : reader.Name);
            return String.Format("read {0} '{1}'", reader.NodeType, content);
        }

        public static string GetPosition(XmlReader reader)
        {
            IXmlLineInfo info = reader as IXmlLineInfo;
            return String.Format("(line {0}, column {1})", info.LineNumber, info.LinePosition);
        }

        public static bool IsElementNamed(XmlReader reader, string name)
        {
            return (reader.NodeType == XmlNodeType.Element && reader.Name.Equals(name));
        }

        public static bool IsEndElementNamed(XmlReader reader, string name)
        {
            return (reader.NodeType == XmlNodeType.EndElement && reader.Name.Equals(name));
        }

        /// <summary>Returns true for 'true' or '1', false for 'false' or '0', otherwise throws exception.</summary>
        public static bool ReadAttributeAsBool(XmlReader reader, string attrName)
        {
            bool b = false;
            int i = 0;

            string attrValue = reader.GetAttribute(attrName);
            if (bool.TryParse(attrValue, out b) || int.TryParse(attrValue, out i))
                return (b || i == 1);
            else
                throw HandleReadAttributeException(null, attrName, "boolean", GetPosition(reader));
        }

        /// <summary>Returns true if value is 'true' or '1', false otherwise.</summary>
        public static bool ReadAttributeAsOptionalBool(XmlReader reader, string attrName)
        {
            bool b; int i;
            string attrValue = reader.GetAttribute(attrName);
            return ((bool.TryParse(attrValue, out b) && b) || (int.TryParse(attrValue, out i) && i == 1));
        }

        public static int ReadAttributeAsInt(XmlReader reader, string attrName)
        {
            string attrValue = reader.GetAttribute(attrName);
            try { return int.Parse(attrValue); }
            catch (Exception e)
            {
                throw HandleReadAttributeException(e, attrName, "integer", GetPosition(reader));
            }
        }

        private static XmlException HandleReadAttributeException(Exception e, string attrName, string attrType, string pos)
        {
            string msg = String.Format("Parsing attribute '{0}' failed! Value has to be a {1}. {2}", attrName, attrType, pos);
            return new XmlException(msg, e);
        }

        public static bool ReadElementAsBool(XmlReader reader, string elemName)
        {
            try
            {
                reader.Read();
                return reader.ReadContentAsBoolean();
            }
            catch (Exception e)
            {
                throw HandleReadElementException(e, elemName, "boolean");
            }
        }

        public static int ReadElementAsInt(XmlReader reader, string elemName)
        {
            try
            {
                reader.Read();
                return reader.ReadContentAsInt();
            }
            catch (Exception e)
            {
                throw HandleReadElementException(e, elemName, "integer");
            }
        }

        public static string ReadElementAsString(XmlReader reader, string elemName)
        {
            try
            {
                reader.Read();
                return reader.ReadContentAsString();
            }
            catch (Exception e)
            {
                throw HandleReadElementException(e, elemName, "string");
            }
        }

        private static XmlException HandleReadElementException(Exception e, string elemName, string elemType)
        {
            string pos = String.Empty;
            if (e is XmlException)
            {
                XmlException xe = e as XmlException;
                pos = String.Format(" (line {0}, column {1})", xe.LineNumber, xe.LinePosition);
            }

            string msg = String.Format("Parsing element '{0}' failed! Value has to be a {1}.{2}", elemName, elemType, pos);
            return new XmlException(msg, e);
        }
    }
}
