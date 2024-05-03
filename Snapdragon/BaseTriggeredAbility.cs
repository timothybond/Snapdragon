namespace Snapdragon
{
    public abstract record BaseTriggeredAbility<TSource, TEvent>() : ITriggeredAbility<TSource>
        where TEvent : Event
    {
        public abstract bool WhenDiscardedOrDestroyed { get; }
        public abstract bool WhenInHand { get; }
        public abstract bool WhenInDeck { get; }

        public EventType EventType => EventTypeMap.Get<TEvent>();

        public Game ProcessEvent(Game game, Event e, TSource source)
        {
            if (e is TEvent typedEvent)
            {
                return ProcessEvent(game, typedEvent, source);
            }

            return game;
        }

        protected abstract Game ProcessEvent(Game game, TEvent e, TSource source);
    }
}
