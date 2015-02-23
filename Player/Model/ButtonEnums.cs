namespace Player.Model
{
    /*
     * Sadly no enums can be defined directly inside an interface in C# :(.
     * So let's share one file for enums used by buttons.
     */

    /// <summary>
    /// How an icon can be displayed inside a button.
    /// </summary>
    public enum DisplayMode
    {
        Default = 0,
        Normal = 0,     // icon gets zoomed as good as it gets, original ratio, nothing clipped (default)
        Zoom,           // icon gets zoomed to fill the button, original ratio, content clipped
        Stretch         // icon gets stretched to fill the button, ratio modified, nothing cropped
    }

    /// <summary>
    /// Supported rotation types for icons. All rotations are clockwise.
    /// </summary>
    public enum Rotation
    {
        None = 0,
        Rotate90 = 0,
        Rotate180 = 0,
        Rotate270 = 0
    }

    /// <summary>
    /// How text can be aligned inside a button.
    /// </summary>
    public enum TextAlignment
    {
        Default = 0,
        Center = 0,
        Top,
        TopRight,
        Right,
        BottomRight,
        Bottom,
        BottomLeft,
        Left,
        TopLeft
    }
}
