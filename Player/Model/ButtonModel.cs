using Player.Model.Action;
using System;

namespace Player.Model
{
    /// <summary>
    /// Represents the model of a button. Buttons are direct child elements of a grid. A grid consists (at least) of one or more buttons.
    /// </summary>
    public interface ButtonModel
    {
        string Id { get; }

        ButtonPosition Position { get; }

        ButtonText Text { get; }

        ButtonIcon Icon { get; }

        GeneralStyle Style { get; set; }

        ActionParameter[] ActionParams { get; }
    }
}
