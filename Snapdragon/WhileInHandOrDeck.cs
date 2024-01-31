namespace Snapdragon
{
    /// <summary>
    /// A <see cref="ITriggeredAbility{Card}"/> that can trigger even in the player's hand or library.
    /// </summary>
    /// <param name="Inner"></param>
    public record WhileInHandOrDeck(TriggeredAbility<Card> Inner) : ITriggeredAbility<Card>
    {
        public bool InHand => true;
        public bool InDeck => true;

        public Game ProcessEvent(Game game, Event e, Card source)
        {
            return Inner.ProcessEvent(game, e, source);
        }
    }
}
