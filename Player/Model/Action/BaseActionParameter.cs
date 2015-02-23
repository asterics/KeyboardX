using System;

namespace Player.Model.Action
{
    abstract class BaseActionParameter : ActionParameter
    {
        public bool RunSynchronous { get; set; }

        protected BaseActionParameter()
        {
            RunSynchronous = true;  // default, asynchronous actions has to be defined explicitly in model
        }
    }
}
