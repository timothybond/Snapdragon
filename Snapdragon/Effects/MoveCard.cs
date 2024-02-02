using Snapdragon.Events;

namespace Snapdragon.Effects
{
    public record MoveCard(Card Card, Column From, Column To) : IEffect
    {
        public Game Apply(Game game)
        {
            var actualCard = game.AllCards.SingleOrDefault(c => c.Id == Card.Id);

            if (actualCard == null)
            {
                return game;
            }

            // TODO: Consider whether this is an appropriate requirement
            if (actualCard.Column != From)
            {
                return game;
            }

            // TODO: handle restrictions on number of cards
            if (game[To][Card.Side].Count >= 4)
            {
                return game;
            }

            var oldLocation = game[From];
            var newLocation = game[To];

            if (game.GetBlockedEffects(oldLocation.Column).Contains(EffectType.MoveFromLocation))
            {
                return game;
            }

            if (game.GetBlockedEffects(newLocation.Column).Contains(EffectType.MoveToLocation))
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
                .WithEvent(new CardMovedEvent(game.Turn, actualCard, From, To));
        }
    }
}
