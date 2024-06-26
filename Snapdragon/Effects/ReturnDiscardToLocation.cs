﻿namespace Snapdragon.Effects
{
    /// <summary>
    /// Returns a discarded <see cref="ICardInstance"/> to play at a specific <see cref="Column"/>.
    /// </summary>
    public record ReturnDiscardToLocation(ICardInstance Card, Column Column) : IEffect
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
                location[actualCard.Side].Count >= Max.CardsPerLocation
                || game.GetBlockedEffects(Column, actualCard.Side).Contains(EffectType.AddCard)
            )
            {
                return game;
            }

            return game.ReturnDiscardToPlayUnsafe(actualCard, Column);
        }
    }
}
