namespace Player.Conf
{
    /// <summary>
    /// Config for <c>Player.Core.Scan</c> can be found here.<para />
    /// Description for properties can be found in XML Schema for config file.
    /// </summary>
    /// <remarks>
    /// The "5 scanning times" can all be found here, even if some not exactly fit. All time values are in milliseconds.
    /// </remarks>
    public interface ScannerConfig
    {
        bool ScannerActive { get; }

        string ScannerType { get; }


        int InitialScanDelay { get; }

        int InputAcceptanceTime { get; }

        int PostAcceptanceDelay { get; }

        int PostInputAcceptanceTime { get; }

        int RepeatAcceptanceDelay { get; }

        int RepeatTime { get; }

        int ScanTime { get; }


        bool StartLeft { get; }

        bool StartTop { get; }

        bool MoveHorizontal { get; }

        int LocalCycleLimit { get; }
    }
}
