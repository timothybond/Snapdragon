using Snapdragon.Events;

namespace Snapdragon.PlayerActions
{
    public record PlayCardAction(Side Side, Card Card, Column Column) : IPlayerAction
    {
        public GameState Apply(GameState initialState)
        {
            // Sanity checks to ensure we should play a card here

            // TODO: Handle effects that limit card play or slots
            var location = initialState[Column];

            if (location[Side].Count >= 4)
            {
                throw new InvalidOperationException($"Tried to play more than 4 cards to {Column} for side {Side}.");
            }

            var player = initialState[Side];

            if (!player.Hand.Contains(Card))
            {
                throw new InvalidOperationException("Tried to play a card that wasn't in the player's hand.");
            }

            if (initialState[Side].Energy < Card.Cost)
            {
                // TODO: This should probably actually log and fail silently.
                throw new InvalidOperationException(
                    $"Tried to play card with cost {Card.Cost}, but remaining energy was {initialState[Side].Energy}.");
            }

            var newPlayerState = player with
            {
                Energy = player.Energy - Card.Cost,
                Hand = player.Hand.Remove(Card)
            };

            var newCard = Card with { State = CardState.PlayedButNotRevealed, Column = Column };

            var newLocationState = location.WithPlayedCard(newCard, Side);

            return initialState
                .WithPlayer(newPlayerState)
                .WithLocation(newLocationState)
                .WithEvent(new CardPlayedEvent(initialState.Turn, newCard));
        }
    }
}
