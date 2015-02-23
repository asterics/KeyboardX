using System;

namespace Player.Model.Action
{
    class SelectActionParameter : BaseActionParameter
    {
        public static readonly string ActionTypeKeyword = "select";


        public string ButtonId { get; private set; }

        public SelectActionParameter(string buttonId)
        {
            ButtonId = buttonId;
        }
    }
}
