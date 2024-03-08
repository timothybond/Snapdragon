﻿namespace Snapdragon.Effects
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

            var column =
                actualCard.Column
                ?? throw new InvalidOperationException(
                    $"{nameof(SwitchCardSide)} was triggered on a card that wasn't in play."
                );

            var location = game[column];

            // TODO: Handle other slot limits
            if (location[actualCard.Side.Other()].Count >= 4)
            {
                return game;
            }

            var modifiedCard = actualCard with { Side = actualCard.Side.Other() };

            location = location.WithRemovedCard(actualCard).WithCard(modifiedCard);

            return game.WithLocation(location);
        }
    }
}
