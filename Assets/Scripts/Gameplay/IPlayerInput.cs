namespace Gameplay
{
    public interface IPlayerInput
    {
        float Horizontal { get; }
        bool Jump { get; }

        bool Fire { get; }
    }
}
