using NLog;
using System;

namespace Player.Core.Trigger
{
    class BaseTrigger : Trigger
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public event EventHandler<TriggerEventArgs> Trigger;


        protected void RaiseTriggerEvent(TriggerEventArgs e)
        {
            logger.Trace("Raising trigger event from {0}...", GetType().Name);

            EventHandler<TriggerEventArgs> handler = Trigger;
            if (handler != null)
                handler(this, e);
        }
    }
}
