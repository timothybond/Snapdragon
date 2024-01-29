namespace Snapdragon
{
    public interface IRevealAbility<T> : IAbility<T>
    {
        GameState Activate(GameState game, T source);
    }
}
