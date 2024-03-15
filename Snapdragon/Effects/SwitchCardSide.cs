using Snapdragon.Events;

namespace Snapdragon.Effects
{
    public record SwitchCardSide(Card Card) : IEffect
    {
        public Game Apply(Game game)
        {
            var actualCard = game.AllCards.SingleOrDefault(c => c.Id == Card.Id);

            if (actualCard == null)
            {
                return game;
            }

            var location = game[actualCard.Column];

            // TODO: Handle other slot limits
            if (location[actualCard.Side.Other()].Count >= Max.CardsPerLocation)
            {
                return game;
            }

            var modifiedCard = actualCard with { Side = actualCard.Side.Other() };

            location = location.WithRemovedCard(actualCard).WithCard(modifiedCard);

            return game.WithLocation(location)
                .WithEvent(new CardSwitchedSidesEvent(modifiedCard, game.Turn));
        }
    }
}
