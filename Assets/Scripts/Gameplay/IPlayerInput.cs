﻿namespace Gameplay
{
    public interface IPlayerInput
    {
        float Horizontal { get; }
        bool Jump { get; }

        bool Fire { get; }

        bool ResetEnergy { get; }
        bool NextTurn { get; }
    }
}
