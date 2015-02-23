using Player.Model;
using System;
using System.Text;

namespace Player.Load.Element
{
    class Style : GeneralStyle
    {
        public string DrawerType { get; set; }

        public string GridBackColor { get; set; }

        public string GridBorderColor { get; set; }

        public string SelectColor { get; set; }

        public string MouseColor { get; set; }

        public int? BorderWidth { get; set; }

        public int? MarginWidth { get; set; }

        public int? GapWidth { get; set; }

        public string ButtonBackColor { get; set; }

        public string ButtonBorderColor { get; set; }

        public string ButtonFontColor { get; set; }

        public int? ButtonFontSize { get; set; }


        public GeneralStyle GetShallowCopy()
        {
            Style copy = new Style();
            copy.InheritFrom(this);
            return copy;
        }

        public void InheritFrom(GeneralStyle style)
        {
            if (DrawerType == null)         DrawerType = style.DrawerType;
            if (GridBackColor == null)      GridBackColor = style.GridBackColor;
            if (GridBorderColor == null)    GridBorderColor = style.GridBorderColor;
            if (SelectColor == null)        SelectColor = style.SelectColor;
            if (MouseColor == null)         MouseColor = style.MouseColor;
            if (BorderWidth == null)        BorderWidth = style.BorderWidth;
            if (MarginWidth == null)        MarginWidth = style.MarginWidth;
            if (GapWidth == null)           GapWidth = style.GapWidth;
            if (ButtonBackColor == null)    ButtonBackColor = style.ButtonBackColor;
            if (ButtonBorderColor == null)  ButtonBorderColor = style.ButtonBorderColor;
            if (ButtonFontColor == null)    ButtonFontColor = style.ButtonFontColor;
            if (ButtonFontSize == null)     ButtonFontSize = style.ButtonFontSize;
        }

        public bool IsComplete()
        {
            return !(DrawerType == null || GridBackColor == null || GridBorderColor == null || SelectColor == null || MouseColor == null || 
                    BorderWidth == null || MarginWidth == null || GapWidth == null || ButtonBackColor == null || ButtonBorderColor == null || 
                    ButtonFontColor == null || ButtonFontSize == null);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(GetType().Name).Append(": {");

            sb.Append("DrawerType = '").Append(DrawerType);
            sb.Append("', GridBackColor = '").Append(GridBackColor);
            sb.Append("', GridBorderColor = '").Append(GridBorderColor);
            sb.Append("', SelectColor = '").Append(SelectColor);
            sb.Append('\n');
            sb.Append("', MouseColor = '").Append(MouseColor);
            sb.Append("', BorderWidth = '").Append(BorderWidth);
            sb.Append("', MarginWidth = '").Append(MarginWidth);
            sb.Append("', GapWidth = '").Append(GapWidth);
            sb.Append('\n');
            sb.Append("', ButtonBackColor = '").Append(ButtonBackColor);
            sb.Append("', ButtonBorderColor = '").Append(ButtonBorderColor);
            sb.Append("', ButtonFontColor = '").Append(ButtonFontColor);
            sb.Append("', ButtonFontSize = '").Append(ButtonFontSize);

            sb.Append("'}");
            return sb.ToString();
        }
    }
}
