using System;

namespace Player.Model
{
    public interface ButtonText
    {
        string Content { get; }

        TextAlignment Alignment { get; }
    }
}
