namespace Snapdragon
{
    /// <summary>
    /// A <see cref="ITriggeredCardAbility{ICard}"/> that can trigger even in the player's library.
    /// </summary>
    /// <param name="Inner"></param>
    public record WhileInDeck<TEvent>(TriggeredCardAbility<TEvent> Inner)
        : BaseTriggeredCardAbility<TEvent>
        where TEvent : Event
    {
        public override bool WhenInHand => false;
        public override bool WhenInDeck => true;
        public override bool WhenDiscardedOrDestroyed => false;

        protected override Game ProcessEvent(Game game, TEvent e, ICard source)
        {
            return Inner.ProcessEvent(game, e, source);
        }
    }
}
