namespace Snapdragon.Effects
{
    /// <summary>
    /// Returns the given Card to the owner's hand (removing it from play/discard/destroyed if applicable).
    ///
    /// Optionally also performs a transformation on the card.
    /// </summary>
    /// <param name="Card"></param>
    public record ReturnCardToHand(ICard Card, Func<CardInstance, CardInstance>? Transform = null)
        : IEffect
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

            var actualCard = inDiscard ?? inDestroyed ?? (ICard)inPlay;

            if (actualCard == null)
            {
                return game;
            }

            if (inDiscard != null)
            {
                player = player with { Discards = player.Discards.RemoveAll(c => c.Id == Card.Id) };
            }

            if (inDestroyed != null)
            {
                player = player with
                {
                    Destroyed = player.Destroyed.RemoveAll(c => c.Id == Card.Id)
                };
            }

            var card = actualCard.ToCardInstance() with { State = CardState.InHand };

            if (this.Transform != null)
            {
                card = this.Transform(card);
            }

            player = player with { Hand = player.Hand.Add(card) };

            game = game.WithPlayer(player);

            if (inPlay != null)
            {
                foreach (var column in All.Columns)
                {
                    if (game[column][Card.Side].Any(c => c.Id == Card.Id))
                    {
                        var location = game[column].WithRemovedCard(inPlay);
                        game = game.WithLocation(location);
                    }
                }
            }

            return game;
        }
    }
}
