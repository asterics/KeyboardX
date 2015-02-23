using System;

namespace Player.Model
{
    public interface ButtonIcon
    {
        string IconPath { get; }

        DisplayMode DisplayMode { get; }

        Rotation Rotation { get; }
    }
}
