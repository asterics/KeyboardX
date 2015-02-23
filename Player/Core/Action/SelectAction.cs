using NLog;
using Player.Core.Scan;
using Player.Model;
using Player.Model.Action;
using System;

namespace Player.Core.Action
{
    /// <summary>
    /// Sets the button state to selected.
    /// Will be mainly for development to have some demo actions.
    /// </summary>
    class SelectAction : BaseAction<SelectActionParameter>
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        protected event EventHandler<SelectionEventArgs> Selection;


        public SelectAction(ActionParameter param, EventHandler<SelectionEventArgs> selectionHandler)
            : base(param)
        {
            Selection += selectionHandler;
        }


        public override void DoAction()
        {
            ButtonGroup selButton = new ButtonGroup(Param.ButtonId);
            RaiseSelectionEvent(new SelectionEventArgs(selButton, ButtonGroup.Empty));
        }

        private void RaiseSelectionEvent(SelectionEventArgs e)
        {
            logger.Trace("Raising selection event...");

            EventHandler<SelectionEventArgs> handler = Selection;
            if (handler != null)
                handler(this, e);
        }
    }
}
