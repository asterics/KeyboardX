using System;

namespace Player.Core.Scan
{
    /// <summary>
    /// A <c>ButtonPressEvent</c> happens when a button is 'virtually' pressed and in response it's actions should be executed.
    /// </summary>
    class ButtonPressEventArgs
    {
        public string ButtonId { get; private set; }

        public ButtonPressEventArgs(string buttonId)
        {
            ButtonId = buttonId;
        }
    }
}
