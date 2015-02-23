using NLog;
using Player.Util;
using System;
using System.Xml;

namespace Player.Load.Parse
{
    /// <summary>
    /// A parent class for all parser implementations.<para />
    /// Combines common reader property and helper functions.
    /// </summary>
    abstract class BaseParser
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        protected XmlReader reader;


        /// <summary>
        /// Resets the whole state of the parser, so that it can safely be reused.<para />
        /// Here only the reader member is nulled (which have to be done in every child class anywhere), but this method also can be overridden, 
        /// in case there is more work to do.
        /// </summary>
        protected virtual void Reset()
        {
            reader = null;
        }

        // used a lot and therefore shortened wrapper methods
        #region UtilWrapper

        protected bool IsElemNamed(string name)
        {
            return XmlUtil.IsElementNamed(reader, name);
        }

        protected bool IsEndElemNamed(string name)
        {
            return XmlUtil.IsEndElementNamed(reader, name);
        }

        protected bool ReadBoolElem(string elemName)
        {
            return XmlUtil.ReadElementAsBool(reader, elemName);
        }

        protected int ReadIntElem(string elemName)
        {
            return XmlUtil.ReadElementAsInt(reader, elemName);
        }

        protected string ReadStringElem(string elemName)
        {
            return XmlUtil.ReadElementAsString(reader, elemName);
        }

        protected string AttrLogMessage(string attrName, string attrValue)
        {
            return XmlUtil.CreateAttributeLogMessage(attrName, attrValue);
        }

        protected string ElemLogMessage()
        {
            return XmlUtil.CreateElementLogMessage(reader);
        }

        /// <summary>Inserts position of <c>XmlReader</c> at placeholder 0.</summary>
        protected string CreatePosMessage(string msg, string opt1 = null)
        {
            return String.Format(msg, XmlUtil.GetPosition(reader), opt1);
        }

        #endregion
    }
}
