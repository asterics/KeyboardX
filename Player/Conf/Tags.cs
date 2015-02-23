namespace Player.Conf
{
    /// <summary>
    /// Holds tag and attribute names of config and keyboard XML files grouped in different subclasses.
    /// </summary>
    public class Tags
    {
        private const string GLOBAL_BUTTON_ELEM = "button";
        private const string GLOBAL_GRID_ELEM = "grid";
        private const string GLOBAL_ID_ATTR = "id";
        private const string GLOBAL_PORT_NAME = "port";

        public static class Config
        {
            public static readonly string ConfigElem = "config";

            public static readonly string GlobalElem = "global";
            public static readonly string SchemaFileNameElem = "schemaFileName";
            public static readonly string SchemaValidationElem = "schemaValidation";
            public static readonly string ClickTriggerActiveElem = "clickTriggerActive";
            public static readonly string ShowMouseChangesElem = "showMouseChanges";
            public static readonly string ButtonPressSynchronous = "buttonPressSynchronous";
            
            public static readonly string NetElem = "net";
            public static readonly string TcpDestinationElem = "tcpDestination";
            public static readonly string TcpDestinationIdAttr = GLOBAL_ID_ATTR;
            public static readonly string TcpDestinationHostElem = "host";
            public static readonly string TcpDestinationPortElem = GLOBAL_PORT_NAME;
            public static readonly string TcpSinkElem = "tcpSink";
            public static readonly string TcpSinkActiveAttr = "active";
            public static readonly string TcpSinkPortAttr = GLOBAL_PORT_NAME;
        }

        public static class Scanner
        {
            public static readonly string ScannerElem = "scanner";
            public static readonly string ScannerActiveElem = "active";
            public static readonly string ScannerTypeElem = "type";
            public static readonly string InitialScanDelayElem = "initialScanDelay";
            public static readonly string InputAcceptanceTimeElem = "inputAcceptanceTime";
            public static readonly string PostAcceptanceDelayElem = "postAcceptanceDelay";
            public static readonly string PostInputAcceptanceTimeElem = "postInputAcceptanceTime";
            public static readonly string RepeatAcceptanceDelayElem = "repeatAcceptanceDelay";
            public static readonly string RepeatTimeElem = "repeatTime";
            public static readonly string ScanTimeElem = "scanTime";
            public static readonly string StartLeftElem = "startLeft";
            public static readonly string StartTopElem = "startTop";
            public static readonly string MoveHorizontalElem = "moveHorizontal";
            public static readonly string LocalCycleLimitElem = "localCycleLimit";
        }

        public static class Style
        {
            public static readonly string StyleElem = "style";
            public static readonly string DrawerTypeElem = "drawer";
            public static readonly string GridBackColorElem = "gridBackColor";
            public static readonly string GridBorderColorElem = "gridBorderColor";
            public static readonly string SelectColorElem = "selectColor";
            public static readonly string MouseColorElem = "mouseColor";
            public static readonly string BorderWidthElem = "borderWidth";
            public static readonly string MarginWidthElem = "marginWidth";
            public static readonly string GapWidthElem = "gapWidth";
            public static readonly string ButtonBackColorElem = "buttonBackColor";
            public static readonly string ButtonBorderColorElem = "buttonBorderColor";
            public static readonly string ButtonFontColorElem = "buttonFontColor";
            public static readonly string ButtonFontSizeElem = "buttonFontSize";
        }

        public static class Keyboard
        {
            public static readonly string KeyboardElem = "keyboard";
            public static readonly string KeyboardIdAttr = GLOBAL_ID_ATTR;

            public static readonly string DefaultElem = "default";
        }

        public static class Grid
        {
            public static readonly string GridElem = GLOBAL_GRID_ELEM;
            public static readonly string GridIdAttr = GLOBAL_ID_ATTR;
            public static readonly string GridDefaultAttr = "default";

            public static readonly string DimensionElem = "dimension";
            public static readonly string DimensionColsAttr = "cols";
            public static readonly string DimensionRowsAttr = "rows";
        }

        public static class Button
        {
            public static readonly string ButtonElem = GLOBAL_BUTTON_ELEM;
            public static readonly string ButtonIdAttr = GLOBAL_ID_ATTR;

            public static readonly string PositionElem = "position";
            public static readonly string PositionXAttr = "x";
            public static readonly string PositionYAttr = "y";
            public static readonly string PositionDimXAttr = "dimX";
            public static readonly string PositionDimYAttr = "dimY";

            public static readonly string CloneElem = "clone";
            public static readonly string CloneButtonAttr = GLOBAL_BUTTON_ELEM;

            public static readonly string IconElem = "icon";
            public static readonly string IconDisplayModeAttr = "display-mode";
            public static readonly string IconRotationAttr = "rotation";

            public static readonly string TextElem = "text";
            public static readonly string TextAlignmentAttr = "alignment";

            public static readonly string StyleElem = "style";
            public static readonly string BackgroundColorElem = "background-color";

            public static readonly string ActionElem = "action";
            public static readonly string ActionTypeAttr = "xsi:type";
            public static readonly string ActionGridSwitchGridAttr = GLOBAL_GRID_ELEM;
            public static readonly string ActionSelectButtonAttr = GLOBAL_BUTTON_ELEM;
            public static readonly string ActionScannerStartAttr = "start";
            public static readonly string ActionTcpHostElem = "host";
            public static readonly string ActionTcpPortElem = "port";
            public static readonly string ActionTcpMessageElem = "message";
            public static readonly string ActionTcpDestinationElem = "destination";
            public static readonly string ActionTcpRefAttr = "ref";
        }
    }
}
