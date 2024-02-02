using Snapdragon.Events;

namespace Snapdragon.Effects
{
    public record DestroyCardInPlay(Card Card) : IEffect
    {
        public Game Apply(Game game)
        {
            if (!Card.Column.HasValue)
            {
                throw new InvalidOperationException("Tried to destroy card with no Column value.");
            }

            var location = game[Card.Column.Value];
            var card = location[Card.Side].SingleOrDefault(c => c.Id == Card.Id);

            if (card == null)
            {
                // TODO: Consider just skipping this, if we run into scenarios where we try to e.g. double-destroy the same thing
                throw new InvalidOperationException(
                    "Tried to destroy card, but it was not found in play at the specified Column/Side."
                );
            }

            var newCardsForSide = location[Card.Side].Remove(card);

            switch (card.Side)
            {
                case Side.Top:
                    location = location with { TopPlayerCards = newCardsForSide };
                    break;
                case Side.Bottom:
                    location = location with { BottomPlayerCards = newCardsForSide };
                    break;
                default:
                    throw new NotImplementedException();
            }

            var player = game[card.Side];
            player = player with
            {
                Destroyed = player.Destroyed.Add(card with { State = CardState.Destroyed })
            };

            return game.WithLocation(location)
                .WithPlayer(player)
                .WithEvent(new CardDestroyedFromPlayEvent(game.Turn, card));
        }
    }
}
