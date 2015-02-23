using Player.Model;
using System;

namespace Player.Core.Trigger
{
    /// <remarks>
    /// Most of the time a trigger event will be empty, but there should be the possibility to trigger a button directly 
    /// (e.g. <see cref="ClickTrigger"/>). Therefore a button id can be sent with the event. 
    /// 
    /// Why don't use the button object itself?
    /// => Because such a trigger could also come over the network sending just the id.
    /// </remarks>
    class TriggerEventArgs : EventArgs
    {
        public static readonly new TriggerEventArgs Empty = new TriggerEventArgs();

        public string ButtonId { get; protected set; }


        private TriggerEventArgs()
        {
            // NOP
        }

        public TriggerEventArgs(string buttonId)
        {
            ButtonId = buttonId;
        }


        public bool ContainsButtonId()
        {
            return !String.IsNullOrEmpty(ButtonId);
        }
    }
}
