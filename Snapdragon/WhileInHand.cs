namespace Snapdragon
{
    /// <summary>
    /// A <see cref="ITriggeredAbility{Card}"/> that can trigger even in the player's hand.
    /// </summary>
    /// <param name="Inner"></param>
    public record WhileInHand<TEvent>(TriggeredAbility<Card, TEvent> Inner)
        : BaseTriggeredAbility<Card, TEvent>
        where TEvent : Event
    {
        public override bool InHand => true;
        public override bool InDeck => false;
        public override bool DiscardedOrDestroyed => false;

        protected override Game ProcessEvent(Game game, TEvent e, Card source)
        {
            return Inner.ProcessEvent(game, e, source);
        }
    }
}
