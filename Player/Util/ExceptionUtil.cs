using System;

namespace Player.Util
{
    /// <summary>
    /// Provides utility methods for exception handling.
    /// </summary>
    static class ExceptionUtil
    {
        /// <summary>
        /// Pretty formats user generated exceptions for logging. 
        /// </summary>
        public static string Format(Exception e)
        {
            if (e.InnerException == null)
                return String.Format("{0}\n{1}", e.Message, e.StackTrace);
            else
                return String.Format("{0}\n{1}", e.Message, e.InnerException);
        }

        /// <summary>
        /// Pretty formats exception with title ahead for logging.
        /// </summary>
        /// <param name="title">Title that should highlight the meaning of the exception.</param>
        public static string Format(string title, Exception e)
        {
            return String.Format("{0}\n{1}", title, e);
        }
    }
}
