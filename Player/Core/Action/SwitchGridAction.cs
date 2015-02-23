using NLog;
using Player.Model.Action;
using System;

namespace Player.Core.Action
{
    /// <summary>
    /// Switch to grid with given id.
    /// </summary>
    class SwitchGridAction : BaseAction<SwitchGridActionParameter>
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        protected event EventHandler<GridSwitchEventArgs> GridSwitch;


        public SwitchGridAction(ActionParameter param, EventHandler<GridSwitchEventArgs> handler)
            : base(param)
        {
            GridSwitch += handler;
        }


        public override void DoAction()
        {
            RaiseGridSwitchEvent(new GridSwitchEventArgs(Param.GridId));
        }

        private void RaiseGridSwitchEvent(GridSwitchEventArgs e)
        {
            logger.Trace("Raising grid switch event...");

            EventHandler<GridSwitchEventArgs> handler = GridSwitch;
            if (handler != null)
                handler(this, e);
        }
    }
}
