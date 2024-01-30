namespace Snapdragon
{
    public interface ITriggeredAbility<T> : IAbility<T>
    {
        Game ProcessEvent(Game game, Event e);
    }
}
