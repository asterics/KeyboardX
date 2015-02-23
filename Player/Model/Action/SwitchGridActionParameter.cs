using System;

namespace Player.Model.Action
{
    class SwitchGridActionParameter : BaseActionParameter
    {
        public static readonly string ActionTypeKeyword = "switch";


        public string GridId { get; private set; }

        public SwitchGridActionParameter(string gridId)
        {
            GridId = gridId;
        }
    }
}
