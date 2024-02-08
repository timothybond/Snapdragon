namespace Snapdragon
{
    public interface IRevealAbility<T>
    {
        Game Activate(Game game, T source);
    }
}
