﻿namespace Snapdragon.TargetFilters
{
    public record SameSide : ICardFilter<Card>
    {
        public bool Applies(Card card, Card source, GameState game)
        {
            return (card.Side == source.Side);
        }
    }
}
