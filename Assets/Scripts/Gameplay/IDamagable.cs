﻿namespace Gameplay
{
    public interface IDamagable
    {
        float GetHealth();
        void TakeDamage(float damage);
        bool IsAlive();
    }
}