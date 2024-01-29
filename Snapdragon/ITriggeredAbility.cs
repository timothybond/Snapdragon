namespace Snapdragon
{
    public interface ITriggeredAbility<T> : IAbility<T>
    {
        public GameState ProcessEvent(Event e, T source);
    }
}
