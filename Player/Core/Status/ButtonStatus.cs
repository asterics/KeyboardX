using System;

namespace Player.Core.Status
{
    /// <summary>
    /// Represents the status of <see cref="Player.Core.Element.Button"/> at runtime (normal, selected, ...).
    /// Introduced because status doesn't fit into <c>Player.Model</c> namespace.
    /// </summary>
    /// <remarks>
    /// Abstract class is used instead of interface, because interfaces doesn't allow inner enums.
    /// </remarks>
    public abstract class ButtonStatus
    {
        [FlagsAttribute]
        public enum Flags : short
        {
            /// <summary>Button is not selected at all.</summary>
            Normal = 0, // default

            /// <summary>Button is selected by scanner.</summary>
            Selected = 1,

            /// <summary>Mouse pointer is over button (which could be visualized).</summary>
            MouseOver = 2,

            /// <summary>Button is touched (which could be visualized).</summary>
            Touched = 4
        }

        public abstract bool Normal { get; protected set; }

        public abstract bool Selected { get; protected set; }

        public abstract bool MouseOver { get; protected set; }

        public abstract bool Touched { get; protected set; }
    }
}
