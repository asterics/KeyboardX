using System;

namespace Player.Model.Action
{
    /// <summary>
    /// Parameter for an action, that opens a new keyboard on a given path and discards old (current) one.
    /// <para/>
    /// Action is not implemented yet!
    /// </summary>
    class OpenKeyboardActionParameter : BaseActionParameter
    {
        public static readonly string ActionTypeKeyword = "open";


        /// <summary>Path to the keyboard file that should be opened.</summary>
        public string Path { get; private set; }

        public OpenKeyboardActionParameter(string path)
        {
            Path = path;
        }
    }
}
