using NLog;
using Player.Model.Action;
using System;

namespace Player.Core.Action
{
    /// <summary>
    /// Base class for all actions.
    /// </summary>
    /// <typeparam name="TParam">Determines <see cref="ActionParameter"/> type for this action.</typeparam>
    abstract class BaseAction<TParam> : IAction where TParam : ActionParameter
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();


        protected TParam Param { get; private set; }

        public bool RunSyncronous
        {
            get { return Param.RunSynchronous; }
        }


        protected BaseAction(ActionParameter param)
        {
            if (!(param is TParam))
                throw new ArgumentException(String.Format("Argument 'param' has to be of type {0} for {1}!", typeof(TParam).Name, GetType().Name));

            Param = (TParam)param;
        }


        public abstract void DoAction();
    }
}
