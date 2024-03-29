namespace Snapdragon.Effects
{
    /// <summary>
    /// Adds a specific card to the given player's hand.
    /// </summary>
    public record AddCardToHand(CardDefinition Definition, Side Side) : IEffect
    {
        public Game Apply(Game game)
        {
            // Note we normally enforce hand-size limit elsewhere.
            // TODO: find a way to centralize this
            if (game[Side].Hand.Count >= Max.HandSize)
            {
                return game;
            }

            return game.WithNewCardInHand(Definition, Side);
        }
    }
}
