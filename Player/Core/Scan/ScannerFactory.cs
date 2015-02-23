using NLog;
using Player.Conf;
using Player.Model;
using System;
using System.Collections.Generic;

namespace Player.Core.Scan
{
    /// <summary>
    /// Creates scanner objects with configuration merged from model and default config.
    /// </summary>
    /// <remarks>
    /// To add a scanner:
    ///  - Add a method that conforms to <see cref="CreateScannerDelegate"/> and returns the scanner.
    ///  - Register this method together with the scanner type keyword in <c>createScanners</c>.
    /// </remarks>
    static class ScannerFactory
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();


        private delegate Scanner CreateScannerDelegate(GridModel grid, ScannerParameter kbScanParams);

        private static Dictionary<string, CreateScannerDelegate> createScanners;


        static ScannerFactory()
        {
            createScanners = new Dictionary<string, CreateScannerDelegate>
            {
                { RowColumnScanner.ScannerTypeKeyword,      CreateRowColumnScanner },
                { ColumnRowScanner.ScannerTypeKeyword,      CreateColumnRowScanner },
                { LinearScanner.ScannerTypeKeyword,         CreateLinearScanner },
                { TestScanner.ScannerTypeKeyword,           CreateTestScanner }
            };
        }


        public static Scanner CreateScanner(GridModel grid, ScannerParameter kbScanParams)
        {
            if (!GetScannerActive(grid, kbScanParams))
            {
                logger.Debug("No scanner is created, because it's explicitly deactivated!");
                return null;
            }

            string type = GetScannerType(grid, kbScanParams);

            try { return createScanners[type](grid, kbScanParams); }
            catch (Exception e)
            {
                string msg;
                if (e is KeyNotFoundException)
                    msg = String.Format("Scanner of type '{0}' isn't implemented!", type);
                else
                    msg = "Unrecognized exception occurred while creating scanner!";

                throw new Exception(msg);
            }
        }

        private static Scanner CreateRowColumnScanner(GridModel grid, ScannerParameter kbScanParams)
        {
            logger.Debug("Creating RowColumnScanner...");

            int isd, pad, piat, st;
            GetScanTimes(grid, kbScanParams, out isd, out pad, out piat, out st);

            bool top = GetStartTop(grid, kbScanParams);
            bool left = GetStartLeft(grid, kbScanParams);
            int lcl = GetLocalCycleLimit(grid, kbScanParams);

            return new RowColumnScanner(grid, isd, pad, piat, st, top, left, lcl);
        }

        private static Scanner CreateColumnRowScanner(GridModel grid, ScannerParameter kbScanParams)
        {
            logger.Debug("Creating ColumnRowScanner...");

            int isd, pad, piat, st;
            GetScanTimes(grid, kbScanParams, out isd, out pad, out piat, out st);

            bool top = GetStartTop(grid, kbScanParams);
            bool left = GetStartLeft(grid, kbScanParams);
            int lcl = GetLocalCycleLimit(grid, kbScanParams);

            return new ColumnRowScanner(grid, isd, pad, piat, st, top, left, lcl);
        }

        private static Scanner CreateLinearScanner(GridModel grid, ScannerParameter kbScanParams)
        {
            logger.Debug("Creating LinearScanner...");

            int isd, pad, piat, st;
            GetScanTimes(grid, kbScanParams, out isd, out pad, out piat, out st);

            bool top = GetStartTop(grid, kbScanParams);
            bool left = GetStartLeft(grid, kbScanParams);
            bool hrz = GetMoveHorizontal(grid, kbScanParams);

            //st = 300;
            //top = false;
            //left = false;
            //hrz = false;

            return new LinearScanner(grid, isd, pad, piat, st, top, left, hrz);
        }

        private static Scanner CreateTestScanner(GridModel grid, ScannerParameter kbScanParams)
        {
            logger.Debug("Creating TestScanner...");

            int isd, pad, piat, st;
            GetScanTimes(grid, kbScanParams, out isd, out pad, out piat, out st);

            //isd = 0;
            //pad = 0;
            //piat = 0;
            //st = 300;

            return new TestScanner(grid, isd, pad, piat, st);
        }

        private static bool GetScannerActive(GridModel grid, ScannerParameter kbScanParams)
        {
            bool active = false;

            if (ScannerActiveIsDefined(grid.ScanParams))
                active = grid.ScanParams.ScannerActive.Value;
            else if (ScannerActiveIsDefined(kbScanParams))
                active = kbScanParams.ScannerActive.Value;
            else
                active = Config.Scanner.ScannerActive;

            return active;
        }

        private static bool ScannerActiveIsDefined(ScannerParameter scanParams)
        {
            return (scanParams != null && scanParams.ScannerActive.HasValue);
        }

        private static string GetScannerType(GridModel grid, ScannerParameter kbScanParams)
        {
            string type;

            if (ScannerTypeIsDefined(grid.ScanParams))
                type = grid.ScanParams.ScannerType;
            else if (ScannerTypeIsDefined(kbScanParams))
                type = kbScanParams.ScannerType;
            else
                type = Config.Scanner.ScannerType;

            return type;
        }

        private static bool ScannerTypeIsDefined(ScannerParameter scanParams)
        {
            return !(scanParams == null || String.IsNullOrEmpty(scanParams.ScannerType)); 
        }

        private static void GetScanTimes(GridModel grid, ScannerParameter kbScanParams, 
                out int initialScanDelay, out int postAcceptanceDelay, out int postInputAcceptanceTime, out int scanTime)
        {
            if (InitialScanDelayIsDefined(grid.ScanParams))
                initialScanDelay = grid.ScanParams.InitialScanDelay.Value;
            else if (InitialScanDelayIsDefined(kbScanParams))
                initialScanDelay = kbScanParams.InitialScanDelay.Value;
            else
                initialScanDelay = Config.Scanner.InitialScanDelay;

            if (PostAcceptanceDelayIsDefined(grid.ScanParams))
                postAcceptanceDelay = grid.ScanParams.PostAcceptanceDelay.Value;
            else if (PostAcceptanceDelayIsDefined(kbScanParams))
                postAcceptanceDelay = kbScanParams.PostAcceptanceDelay.Value;
            else
                postAcceptanceDelay = Config.Scanner.PostAcceptanceDelay;

            if (PostInputAcceptanceTimeIsDefined(grid.ScanParams))
                postInputAcceptanceTime = grid.ScanParams.PostInputAcceptanceTime.Value;
            else if (PostInputAcceptanceTimeIsDefined(kbScanParams))
                postInputAcceptanceTime = kbScanParams.PostInputAcceptanceTime.Value;
            else
                postInputAcceptanceTime = Config.Scanner.PostInputAcceptanceTime;

            if (ScanTimeIsDefined(grid.ScanParams))
                scanTime = grid.ScanParams.ScanTime.Value;
            else if (ScanTimeIsDefined(kbScanParams))
                scanTime = kbScanParams.ScanTime.Value;
            else
                scanTime = Config.Scanner.ScanTime;
        }

        private static bool InitialScanDelayIsDefined(ScannerParameter scanParams)
        {
            return (scanParams != null && scanParams.InitialScanDelay.HasValue);
        }

        private static bool PostAcceptanceDelayIsDefined(ScannerParameter scanParams)
        {
            return (scanParams != null && scanParams.PostAcceptanceDelay.HasValue);
        }

        private static bool PostInputAcceptanceTimeIsDefined(ScannerParameter scanParams)
        {
            return (scanParams != null && scanParams.PostInputAcceptanceTime.HasValue);
        }

        private static bool ScanTimeIsDefined(ScannerParameter scanParams)
        {
            return (scanParams != null && scanParams.ScanTime.HasValue);
        }

        private static bool GetStartLeft(GridModel grid, ScannerParameter kbScanParams)
        {
            bool left;

            if (StartLeftIsDefined(grid.ScanParams))
                left = grid.ScanParams.StartLeft.Value;
            else if (StartLeftIsDefined(kbScanParams))
                left = kbScanParams.StartLeft.Value;
            else
                left = Config.Scanner.StartLeft;

            return left;
        }

        private static bool StartLeftIsDefined(ScannerParameter scanParams)
        {
            return (scanParams != null && scanParams.StartLeft.HasValue);
        }

        private static bool GetStartTop(GridModel grid, ScannerParameter kbScanParams)
        {
            bool top;

            if (StartTopIsDefined(grid.ScanParams))
                top = grid.ScanParams.StartTop.Value;
            else if (StartTopIsDefined(kbScanParams))
                top = kbScanParams.StartTop.Value;
            else
                top = Config.Scanner.StartTop;

            return top;
        }

        private static bool StartTopIsDefined(ScannerParameter scanParams)
        {
            return (scanParams != null && scanParams.StartTop.HasValue);
        }

        private static bool GetMoveHorizontal(GridModel grid, ScannerParameter kbScanParams)
        {
            bool hrz;

            if (MoveHorizontalIsDefined(grid.ScanParams))
                hrz = grid.ScanParams.MoveHorizontal.Value;
            else if (MoveHorizontalIsDefined(kbScanParams))
                hrz = kbScanParams.MoveHorizontal.Value;
            else
                hrz = Config.Scanner.MoveHorizontal;

            return hrz;
        }

        private static bool MoveHorizontalIsDefined(ScannerParameter scanParams)
        {
            return (scanParams != null && scanParams.MoveHorizontal.HasValue);
        }

        private static int GetLocalCycleLimit(GridModel grid, ScannerParameter kbScanParams)
        {
            int limit;

            if (LocalCycleLimitIsDefined(grid.ScanParams))
                limit = grid.ScanParams.LocalCycleLimit.Value;
            else if (LocalCycleLimitIsDefined(kbScanParams))
                limit = kbScanParams.LocalCycleLimit.Value;
            else
                limit = Config.Scanner.LocalCycleLimit;

            return limit;
        }

        private static bool LocalCycleLimitIsDefined(ScannerParameter scanParams)
        {
            return (scanParams != null && scanParams.LocalCycleLimit.HasValue);
        }
    }
}
