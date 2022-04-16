namespace Gameplay.Player
{
    public interface IPlayerInput
    {
        float Horizontal { get; }
        bool IsJumping { get; }
    }
}
