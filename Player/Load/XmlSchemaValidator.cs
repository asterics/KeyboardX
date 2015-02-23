using NLog;
using Player.Conf;
using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace Player.Load
{
    /// <summary>
    /// Validates keyboard file with fixed XML Schema Definition.
    /// </summary>
    class XmlSchemaValidator
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();


        private bool validated;
        private readonly object validatedLock = new object();

        private bool valid;

        private StringBuilder errorsWarnings;


        public XmlSchemaValidator()
        {
            validated = false;
            valid = false;
            errorsWarnings = new StringBuilder();
        }


        public void Validate(Stream keyboardStream)
        {
            lock (validatedLock)
            {
                if (validated)
                    throw new InvalidOperationException("Calling Validate(..) is only allowed once per instance!");

                try
                {
                    XmlReader reader = XmlReader.Create(keyboardStream, CreateValidationSettings());

                    valid = true;
                    while (reader.Read()) ;  // parse and validate XML file
                    validated = true;
                }
                catch (XmlException e)
                {
                    string msg = "Something is wrong with the XML structure of the keyboard!";
                    throw new LoaderException(msg, e);
                }
            }

            /* Another possible way of validating, but there we didn't get valid line and column information:
                XmlDocument kbd = new XmlDocument();
                kbd.Load(keyboardStream);
                kbd.Schemas.Add(null, schemaFilePath);

                valid = true;
                kbd.Validate(OnValidationEvent);
                validated = true;
            */
        }

        private XmlReaderSettings CreateValidationSettings()
        {
            string schemaFilePath = Config.Global.SchemaFileName;  // schema file has to be in main directory

            try
            {
                XmlReaderSettings settings = new XmlReaderSettings();

                settings.ValidationFlags = XmlSchemaValidationFlags.ProcessIdentityConstraints |
                                            XmlSchemaValidationFlags.ReportValidationWarnings;

                settings.ValidationType = ValidationType.Schema;
                settings.Schemas.Add(null, schemaFilePath);
                settings.ValidationEventHandler += OnValidationEvent;

                return settings;
            }
            catch (FileNotFoundException)
            {
                string msg = String.Format("Schema file for validation couldn't be found! ({0})", schemaFilePath);
                throw new LoaderException(msg);
            }
        }

        /// <returns><c>True</c> if no errors or warnings occurred.</returns>
        public bool IsValid()
        {
            lock (validatedLock)
            {
                if (!validated)
                    throw new InvalidOperationException("Calling IsKeyboardValid() makes only sense after calling Validate(..)!");
                else
                    return valid;
            }
        }

        public string GetErrorsAndWarnings()
        {
            lock (validatedLock)
            {
                if (!validated)
                    throw new InvalidOperationException("Calling GetErrorMessage() makes only sense after calling Validate(..)!");
                else
                    return errorsWarnings.ToString();
            }
        }

        protected void OnValidationEvent(Object sender, ValidationEventArgs e)
        {
            valid = false;

            string msg = String.Format("{0} (line {1}, column {2})", e.Message, e.Exception.LineNumber, e.Exception.LinePosition);

            switch (e.Severity)
            {
                case XmlSeverityType.Warning:
                    logger.Debug("XML-Warning: {0}", msg);
                    errorsWarnings.Append("Warning: ");
                    errorsWarnings.AppendLine(msg);
                    break;
                case XmlSeverityType.Error:
                    logger.Debug("XML-Error: {0}", msg);
                    errorsWarnings.Append("Error: ");
                    errorsWarnings.AppendLine(msg);
                    break;
            }
        }
    }
}
