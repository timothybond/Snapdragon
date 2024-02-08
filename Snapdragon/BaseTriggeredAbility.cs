namespace Snapdragon
{
    public interface ITriggeredAbility<TSource>
    {
        bool InHand { get; }
        bool InDeck { get; }
        bool DiscardedOrDestroyed { get; }

        Game ProcessEvent(Game game, Event e, TSource source);
    }

    public abstract record BaseTriggeredAbility<TSource, TEvent> : ITriggeredAbility<TSource>
        where TEvent : Event
    {
        public abstract bool InHand { get; }
        public abstract bool InDeck { get; }
        public abstract bool DiscardedOrDestroyed { get; }

        public Game ProcessEvent(Game game, Event e, TSource source)
        {
            if (e is TEvent specificEvent)
            {
                return ProcessEvent(game, specificEvent, source);
            }

            return game;
        }

        protected abstract Game ProcessEvent(Game game, TEvent e, TSource source);
    }
}
