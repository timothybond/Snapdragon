namespace Snapdragon
{
    /// <summary>
    /// A <see cref="ITriggeredAbility{Card}"/> that can trigger even in the player's hand.
    /// </summary>
    /// <param name="Inner"></param>
    public record WhileInHand(TriggeredAbility<Card> Inner) : ITriggeredAbility<Card>
    {
        public bool InHand => true;
        public bool InDeck => false;

        public Game ProcessEvent(Game game, Event e, Card source)
        {
            return Inner.ProcessEvent(game, e, source);
        }
    }
}
