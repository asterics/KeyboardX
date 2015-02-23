using NLog;
using Player.Model.Action;
using System;

namespace Player.Core.Action
{
    /// <summary>
    /// Logs a message.
    /// Will be mainly for development to have some demo actions.
    /// </summary>
    class LogAction : BaseAction<LogActionParameter>
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();


        public LogAction(ActionParameter param)
            : base(param)
        {
            // NOP
        }


        public override void DoAction()
        {
            logger.Info(Param.Message);
        }
    }
}
