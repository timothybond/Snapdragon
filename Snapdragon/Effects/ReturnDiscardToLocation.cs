using Snapdragon.Events;

namespace Snapdragon.Effects
{
    /// <summary>
    /// Returns a discarded <see cref="ICard"/> to play at a specific <see cref="Column"/>.
    /// </summary>
    public record ReturnDiscardToLocation(ICard Card, Column Column) : IEffect
    {
        public Game Apply(Game game)
        {
            var player = game[Card.Side];

            var actualCard = player.Discards.SingleOrDefault(c => c.Id == Card.Id);
            if (actualCard == null)
            {
                return game;
            }

            var location = game[Column];

            if (
                location[Card.Side].Count >= Max.CardsPerLocation
                || game.GetBlockedEffects(Column, Card.Side).Contains(EffectType.AddCard)
            )
            {
                return game;
            }

            location = location.WithCard(actualCard with { State = CardState.InPlay });

            player = player with { Discards = player.Discards.RemoveAll(c => c.Id == Card.Id) };

            return game.WithPlayer(player)
                .WithLocation(location)
                .WithEvent(new CardReturnedToPlay(actualCard, Column, game.Turn));
        }
    }
}
