namespace Snapdragon
{
    /// <summary>
    /// A <see cref="ITriggeredCardAbility{Card}"/> that can trigger even in the player's hand.
    /// </summary>
    /// <param name="Inner"></param>
    public record WhileInHand<TEvent>(TriggeredCardAbility<TEvent> Inner)
        : BaseTriggeredCardAbility<TEvent>
        where TEvent : Event
    {
        public override bool WhenInHand => true;
        public override bool WhenInDeck => false;
        public override bool WhenDiscardedOrDestroyed => false;

        protected override Game ProcessEvent(Game game, TEvent e, ICardInstance source)
        {
            return Inner.ProcessEvent(game, e, source);
        }
    }
}
