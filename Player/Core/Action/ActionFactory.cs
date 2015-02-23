using Player.Core.Net;
using Player.Core.Scan;
using Player.Model.Action;
using System;
using ActionDelegate = System.Action;

namespace Player.Core.Action
{
    /// <summary>
    /// Creates <see cref="IAction"/> object for given <see cref="ActionParameter"/>.
    /// </summary>
    static class ActionFactory
    {
        public static event EventHandler<SelectionEventArgs> SelectionHandler;

        public static event EventHandler<GridSwitchEventArgs> GridSwitchHandler;

        public static TcpConnectionPool ConnectionPool { private get; set; }

        public static ActionDelegate StartScanner { private get; set; }
        public static ActionDelegate StopScanner { private get; set; }


        public static IAction CreateAction(ActionParameter param)
        {
            if (param is SwitchGridActionParameter)
                return new SwitchGridAction(param, GridSwitchHandler);
            else if (param is TcpActionParameter)
                return new TcpAction(param, ConnectionPool);
            else if (param is TTSActionParameter)
                return new TTSAction(param);
            else if (param is LogActionParameter)
                return new LogAction(param);
            else if (param is ScannerActionParameter)
                return new ScannerAction(param, StartScanner, StopScanner);
            else if (param is SelectActionParameter)
                return new SelectAction(param, SelectionHandler);
            else if (param is TimeActionParameter)
                return new TimeAction(param);
            else
                throw new NotImplementedException(String.Format("Action for parameter type {0} is not implemented!", param.GetType().Name));
        }
    }
}
