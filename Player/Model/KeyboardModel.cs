using System;

namespace Player.Model
{
    /// <summary>
    /// Represents the model of a keyboard. A keyboard is the root element inside a keyboard file. A keyboard consists of one or more grids.
    /// </summary>
    public interface KeyboardModel
    {
        string Name { get; }

        GridModel this[string gridId]  { get; }

        GridModel DefaultGrid { get; }

        ScannerParameter ScanParams { get; }

        GeneralStyle Style { get; set; }
    }
}
