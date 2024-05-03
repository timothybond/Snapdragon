namespace Snapdragon
{
    public interface ITriggeredAbility<TContext>
    {
        EventType EventType { get; }

        Game ProcessEvent(Game game, Event e, TContext context);

        bool WhenDiscardedOrDestroyed { get; }
        bool WhenInHand { get; }
        bool WhenInDeck { get; }
    }
}
