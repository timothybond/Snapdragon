namespace Snapdragon
{
    public abstract record BaseTriggeredCardAbility<TEvent> : ITriggeredCardAbility
        where TEvent : Event
    {
        public abstract bool InHand { get; }
        public abstract bool InDeck { get; }
        public abstract bool DiscardedOrDestroyed { get; }

        public Game ProcessEvent(Game game, Event e, ICard source)
        {
            if (e is TEvent specificEvent)
            {
                return ProcessEvent(game, specificEvent, source);
            }

            return game;
        }

        protected abstract Game ProcessEvent(Game game, TEvent e, ICard source);
    }
}
