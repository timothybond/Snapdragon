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
                return game;
            }

            location = location.WithoutCard(card);

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
