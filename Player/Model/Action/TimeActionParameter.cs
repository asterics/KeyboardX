using System;

namespace Player.Model.Action
{
    class TimeActionParameter : BaseActionParameter
    {
        public static readonly string ActionTypeKeyword = "time";


        public TimeSpan Timeout { get; private set; }

        public TimeActionParameter(TimeSpan timeout)
        {
            RunSynchronous = true;  // asyncronous TimeAction makes no sense
            Timeout = timeout;
        }
    }
}
