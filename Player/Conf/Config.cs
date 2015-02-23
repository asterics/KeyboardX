using NLog;
using Player.Conf.Build;
using Player.Model;
using Player.Util;
using System;

namespace Player.Conf
{
    /// <summary>
    /// Static base class for project wide configuration.
    /// <para />
    /// This should be used as starting point for getting configuration values. <c>Build()</c> has to be called first.
    /// Values shouldn't be cached for long, cause reloading could possibly take place.
    /// </summary>
    /// TODOs:
    ///  TODO 5: [conf] validate config after building, especially scanning times!
    public static class Config
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>The Keyboards directory in the repository, where test and showroom are subdirectories.</summary>
        public static string KeyboardsBaseDirectory;

        /// <summary>The icon directory in the repository.</summary>
        public static string IconBaseDirectory;

        private static GlobalConfig _global;
        public static GlobalConfig Global
        {
            get
            {
                if (_global == null)
                    ThrowNotInitializedException(typeof(GlobalConfig));

                return _global;
            }

            set { _global = value; }
        }

        private static ScannerConfig _scanner;
        public static ScannerConfig Scanner
        {
            get
            {
                if (_scanner == null)
                    ThrowNotInitializedException(typeof(ScannerConfig));

                return _scanner;
            }

            set { _scanner = value; }
        }

        // Using GeneralStyle class for now. Maybe we should better implement an own StyleConfig class.
        private static GeneralStyle _style;
        public static GeneralStyle Style
        {
            get
            {
                if (_style == null)
                    ThrowNotInitializedException(typeof(GeneralStyle));

                return _style;
            }

            set { _style = value; }
        }


        /// <summary>
        /// Builds configuration, i.e. load values from config file and merge them with hard coded values.
        /// Have to be called before accessing static configuration properties.
        /// <para />
        /// Why not call this via static constructor?
        /// Because if an error occurs, not even logger is available yet.
        /// </summary>
        public static void Build()
        {
            int start = Environment.TickCount;

            ConfigBuilder builder = new ConfigBuilder();

            ConfigException ce = null;
            try { builder.BuildConfig(); }
            catch (ConfigException e)  // log exceptions thrown in Player.Conf.Build only here
            {
                logger.Error(ExceptionUtil.Format(e));
                ce = e;
            }
                
            Global = builder.GlobalConfig;
            Scanner = builder.ScannerConfig;
            Style = builder.StyleConfig;

            logger.Trace("Build() needed {0}ms", (Environment.TickCount - start));

            if (ce != null)  // signal error
                throw ce;
        }

        private static void ThrowNotInitializedException(Type config)
        {
            string msg = String.Format("{0} hadn't been initialized yet! Something went wrong while setting up {1}.", config.Name, typeof(Config).Name);
            logger.Error(msg);
            throw new NotInitializedException(msg);
        }
    }
}
