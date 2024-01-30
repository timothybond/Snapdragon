namespace Snapdragon
{
    public interface IRevealAbility<T> : IAbility<T>
    {
        Game Activate(Game game, T source);
    }
}
