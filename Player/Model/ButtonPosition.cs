using System;

namespace Player.Model
{
    /// <remarks>
    /// A button can only be rectangular, if there should be other forms, you have to use more buttons which reference each other.
    /// </remarks>
    public interface ButtonPosition
    {
        int X { get; }

        int Y { get; }

        /// <summary>The dimension in x coordinates, i.e. how many columns are allocated.</summary>
        int DimX { get; }

        /// <summary>The dimension in y coordinates, i.e. how many rows are allocated.</summary>
        int DimY { get; }
    }
}
