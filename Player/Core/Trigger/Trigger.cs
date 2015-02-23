using System;

namespace Player.Core.Trigger
{
    /// <summary>
    /// A trigger can happen by various sources, like keyboard, mouse, network, etc..
    /// An implementing class should signal a trigger by raising the event.
    /// </summary>
    interface Trigger
    {
        event EventHandler<TriggerEventArgs> Trigger;
    }
}
