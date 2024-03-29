namespace Snapdragon
{
    public abstract record BaseTriggeredCardAbility : ITriggeredAbility<ICardInstance>, ISpecialCardTrigger
    {
        public abstract bool WhenInHand { get; }
        public abstract bool WhenInDeck { get; }
        public abstract bool WhenDiscardedOrDestroyed { get; }

        public abstract Game ProcessEvent(Game game, Event e, ICardInstance source);
    }

    public abstract record BaseTriggeredCardAbility<TEvent> : BaseTriggeredCardAbility
        where TEvent : Event
    {
        public override Game ProcessEvent(Game game, Event e, ICardInstance source)
        {
            if (e is TEvent specificEvent)
            {
                return ProcessEvent(game, specificEvent, source);
            }

            return game;
        }

        protected abstract Game ProcessEvent(Game game, TEvent e, ICardInstance source);
    }
}
