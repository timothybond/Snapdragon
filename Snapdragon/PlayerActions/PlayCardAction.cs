using Snapdragon.Events;

namespace Snapdragon.PlayerActions
{
    public record PlayCardAction(Side Side, CardInstance Card, Column Column) : IPlayerAction
    {
        public Game Apply(Game game)
        {
            // Sanity checks to ensure we should play a card here

            // TODO: Handle effects that limit card play or slots
            var location = game[Column];

            if (location[Side].Count >= Max.CardsPerLocation)
            {
                throw new InvalidOperationException(
                    $"Tried to play more than 4 cards to {Column} for side {Side}."
                );
            }

            if (Card.PlayRestriction?.IsBlocked(game, Column, Card) ?? false)
            {
                throw new InvalidOperationException(
                    "Tried to play a card with a play restriction that blocks it."
                );
            }

            if (game.GetBlockedEffects(Column, Side).Contains(EffectType.PlayCard))
            {
                throw new InvalidOperationException(
                    $"The 'PlayCard' effect type is blocked for side {Side}, Column {Column}."
                );
            }

            var player = game[Side];

            if (!player.Hand.Contains(Card))
            {
                throw new InvalidOperationException(
                    "Tried to play a card that wasn't in the player's hand."
                );
            }

            if (game[Side].Energy < Card.Cost)
            {
                throw new InvalidOperationException(
                    $"Tried to play card with cost {Card.Cost}, but remaining energy was {game[Side].Energy}."
                );
            }

            // TODO: Consider making an IEffect for this
            var newPlayerState = player with
            {
                Energy = player.Energy - Card.Cost,
                Hand = player.Hand.Remove(Card)
            };

            var newCard = Card with { State = CardState.PlayedButNotRevealed, Column = Column };

            var newLocationState = location.WithCard(newCard);

            return game.WithPlayer(newPlayerState)
                .WithLocation(newLocationState)
                .WithEvent(new CardPlayedEvent(game.Turn, newCard));
        }

        public override string ToString()
        {
            return $"[{Side}]: Play {Card.Name} ({Card.Id}) at {Column}.";
        }
    }
}
