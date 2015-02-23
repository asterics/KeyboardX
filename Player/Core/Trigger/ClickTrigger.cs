using NLog;
using Player.Core.Gui;
using System;

namespace Player.Core.Trigger
{
    /// <summary>
    /// Triggers when a button has been clicked.
    /// </summary>
    class ClickTrigger : BaseTrigger
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();


        public void OnClick(Object sender, ClickEventArgs ce)
        {
            TriggerEventArgs te;

            if (String.IsNullOrEmpty(ce.ButtonId))
            {
                logger.Trace("Empty click event received.");
                te = TriggerEventArgs.Empty;
            }
            else
            {
                logger.Trace("Click event for button '{0}' received.", ce.ButtonId);
                te = new TriggerEventArgs(ce.ButtonId);
            }

            RaiseTriggerEvent(te);
        }
    }
}
