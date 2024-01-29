namespace Snapdragon
{
    public interface ITriggeredAbility<T> : IAbility<T>
    {
        GameState ProcessEvent(GameState game, Event e);
    }
}
