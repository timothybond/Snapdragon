namespace Snapdragon.Effects
{
    /// <summary>
    /// Returns the given Card to the owner's hand (removing it from play/discard/destroyed if applicable).
    ///
    /// Optionally also performs a transformation on the card.
    /// </summary>
    /// <param name="Card"></param>
    public record ReturnCardToHand(Card Card, Func<Card, Card>? Transform = null) : IEffect
    {
        public Game Apply(Game game)
        {
            var player = game[Card.Side];

            // TODO: Determine if there's any scenarios where this isn't correct
            if (player.Hand.Count >= 7)
            {
                return game;
            }

            if (player.Discards.Any(c => c.Id == Card.Id))
            {
                player = player with { Discards = player.Discards.RemoveAll(c => c.Id == Card.Id) };
            }

            if (player.Destroyed.Any(c => c.Id == Card.Id))
            {
                player = player with
                {
                    Destroyed = player.Destroyed.RemoveAll(c => c.Id == Card.Id)
                };
            }

            var card = Card with { State = CardState.InHand };

            if (this.Transform != null)
            {
                card = this.Transform(card);
            }

            player = player with { Hand = player.Hand.Add(card) };

            game = game.WithPlayer(player);

            foreach (var column in All.Columns)
            {
                if (game[column][Card.Side].Any(c => c.Id == Card.Id))
                {
                    var location = game[column].WithRemovedCard(Card);
                    game = game.WithLocation(location);
                }
            }

            return game;
        }
    }
}
