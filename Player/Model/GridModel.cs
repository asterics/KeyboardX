using System;
using System.Collections.Generic;

namespace Player.Model
{
    /// <summary>
    /// Represents the model of a grid. Grids are direct child elements of a keyboard. A keyboard consists (at least) of one or more grids.
    /// </summary>
    public interface GridModel : IEnumerable<ButtonModel>
    {
        string Id { get; }

        int Cols { get; }

        int Rows { get; }

        /// <summary>Number of columns that contain at least one button.</summary>
        int NonEmptyCols { get; }

        /// <summary>Number of rows that contain at least one button.</summary>
        int NonEmptyRows { get; }

        ButtonModel this[int x, int y] { get; }

        ButtonModel this[string buttonId] { get; }

        ScannerParameter ScanParams { get; }

        GeneralStyle Style { get; set; }

        
        ButtonGroup GetButtonsForCol(int x);

        ButtonGroup GetButtonsForRow(int y);

        ButtonGroup GetButtonsForArea(int index);
    }
}
