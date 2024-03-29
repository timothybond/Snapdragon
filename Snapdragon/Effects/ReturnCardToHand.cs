namespace Snapdragon.Effects
{
    /// <summary>
    /// Returns the given Card to the owner's hand (removing it from play/discard/destroyed if applicable).
    ///
    /// Optionally also performs a transformation on the card.
    /// </summary>
    /// <param name="Card"></param>
    public record ReturnCardToHand(ICardInstance Card, Func<CardBase, CardBase>? Transform = null) : IEffect
    {
        public Game Apply(Game game)
        {
            var player = game[Card.Side];

            // TODO: Determine if there's any scenarios where this isn't correct
            if (player.Hand.Count >= Max.HandSize)
            {
                return game;
            }

            var inDiscard = player.Discards.SingleOrDefault(c => c.Id == Card.Id);
            var inDestroyed = player.Destroyed.SingleOrDefault(c => c.Id == Card.Id);
            var inPlay = game.AllCards.SingleOrDefault(c => c.Id == Card.Id);

            var total =
                (inDiscard != null ? 1 : 0)
                + (inDestroyed != null ? 1 : 0)
                + (inPlay != null ? 1 : 0);

            if (total > 1)
            {
                throw new InvalidOperationException(
                    $"Found card {Card.Name} ({Card.Id}) in multiple states."
                );
            }

            var actualCard = inDiscard ?? inDestroyed ?? inPlay;

            if (actualCard == null)
            {
                return game;
            }

            return game.ReturnCardToHand(actualCard);
        }
    }
}
