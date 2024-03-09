using Snapdragon.Events;

namespace Snapdragon.Effects
{
    /// <summary>
    /// Moves a <see cref="Card"/> left by one location.
    ///
    /// This is effectively very similar to the <see cref="MoveCard"/> effect,
    /// except for the following:
    ///
    /// - The card is always moved from its current column to the one immediately
    ///   to the left, assuming this is possible.
    /// - There is no check for move abilities, because it is assumed this comes
    ///   from another effect (like setting "Forced" on <see cref="MoveCard"/>).
    /// </summary>
    public record MoveCardLeft(Card Card) : IEffect
    {
        public Game Apply(Game game)
        {
            var actualCard = game.AllCards.SingleOrDefault(c => c.Id == Card.Id);

            if (actualCard == null)
            {
                return game;
            }

            Column destination;
            var from = actualCard.Column;

            switch (from)
            {
                case Column.Right:
                    destination = Column.Middle;
                    break;
                case Column.Middle:
                    destination = Column.Left;
                    break;
                case Column.Left:
                    return game;
                default:
                    throw new NotImplementedException();
            }

            // TODO: handle restrictions on number of cards
            if (game[destination][Card.Side].Count >= 4)
            {
                return game;
            }

            // Note if Column is null, we would have returned in the switch case above
            var oldLocation = game[from];
            var newLocation = game[destination];

            if (
                game.GetBlockedEffects(oldLocation.Column, actualCard.Side)
                    .Contains(EffectType.MoveFromLocation)
            )
            {
                return game;
            }

            if (
                game.GetBlockedEffects(newLocation.Column, actualCard.Side)
                    .Contains(EffectType.MoveToLocation)
            )
            {
                return game;
            }

            if (game.GetBlockedEffects(actualCard).Contains(EffectType.MoveCard))
            {
                return game;
            }

            oldLocation = oldLocation.WithRemovedCard(actualCard);
            actualCard = actualCard with { Column = newLocation.Column };
            newLocation = newLocation.WithCard(actualCard);

            return game.WithLocation(oldLocation)
                .WithLocation(newLocation)
                .WithEvent(new CardMovedEvent(game.Turn, actualCard, from, destination));
        }
    }
}
