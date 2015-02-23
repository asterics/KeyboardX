using System;

namespace Player.Model
{
    /// <summary>
    /// Generic scanner model, contains all config for a scanner. 
    /// </summary>
    public interface ScannerParameter
    {
        bool? ScannerActive { get; }

        string ScannerType { get; }


        int? InitialScanDelay { get; }

        int? InputAcceptanceTime { get; }

        int? PostAcceptanceDelay { get; }

        int? PostInputAcceptanceTime { get; }

        int? ScanTime { get; }


        bool? StartLeft { get; }

        bool? StartTop { get; }

        bool? MoveHorizontal { get; }


        int? LocalCycleLimit { get; }
    }
}
