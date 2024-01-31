namespace Snapdragon
{
    public interface ITriggeredAbility<T> : IAbility<T>
    {
        bool InHand { get; }
        bool InDeck { get; }
        bool DiscardedOrDestroyed { get; }

        Game ProcessEvent(Game game, Event e, T source);
    }
}
