namespace Combat
{
    public interface IDamagable
    {
        int GetHealth();
        void TakeDamage(int damage);
        bool IsAlive();
    }
}
