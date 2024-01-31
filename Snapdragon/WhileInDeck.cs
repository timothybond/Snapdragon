namespace Snapdragon
{
    /// <summary>
    /// A <see cref="ITriggeredAbility{Card}"/> that can trigger even in the player's library.
    /// </summary>
    /// <param name="Inner"></param>
    public record WhileInDeck(TriggeredAbility<Card> Inner) : ITriggeredAbility<Card>
    {
        public bool InHand => false;
        public bool InDeck => true;
        public bool DiscardedOrDestroyed => false;

        public Game ProcessEvent(Game game, Event e, Card source)
        {
            return Inner.ProcessEvent(game, e, source);
        }
    }
}
