namespace Snapdragon
{
    /// <summary>
    /// A <see cref="ITriggeredAbility{Card}"/> that can trigger even in the player's hand or library.
    /// </summary>
    /// <param name="Inner"></param>
    public record WhileInHandOrDeck<TEvent>(TriggeredAbility<Card, TEvent> Inner)
        : BaseTriggeredAbility<Card, TEvent>
        where TEvent : Event
    {
        public override bool InHand => true;
        public override bool InDeck => true;
        public override bool DiscardedOrDestroyed => false;

        protected override Game ProcessEvent(Game game, TEvent e, Card source)
        {
            return Inner.ProcessEvent(game, e, source);
        }
    }
}
