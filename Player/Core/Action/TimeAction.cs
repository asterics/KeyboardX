using NLog;
using Player.Model.Action;
using System;
using System.Threading;

namespace Player.Core.Action
{
    /// <summary>
    /// Simply waits for a given time.
    /// Will be mainly for development to have some demo actions.
    /// </summary>
    class TimeAction : BaseAction<TimeActionParameter>
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();


        public TimeAction(ActionParameter param)
            :base(param)
        {
            // NOP
        }


        public override void DoAction()
        {
            Thread.Sleep(Param.Timeout);
            logger.Info("{0} finished.", GetType().Name);
        }
    }
}
