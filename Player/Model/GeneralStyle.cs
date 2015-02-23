using System;

namespace Player.Model
{
    /// <summary>
    /// A general style representation used from application to button level.<para />
    /// Maybe it would be better to put this in a more hierarchical structure instead of this flat one. This was due massive lack of time.
    /// </summary>
    public interface GeneralStyle
    {
        /* Grid specific. */

        string DrawerType { get; }

        string GridBackColor { get; }

        string GridBorderColor { get; }

        string SelectColor { get; }

        string MouseColor { get; }

        int? BorderWidth { get; }

        int? MarginWidth { get; }

        int? GapWidth { get; }

        /* Button specific. */

        string ButtonBackColor { get; }

        string ButtonBorderColor { get; }

        string ButtonFontColor { get; }

        int? ButtonFontSize { get; }


        GeneralStyle GetShallowCopy();

        void InheritFrom(GeneralStyle style);

        bool IsComplete();
    }
}
