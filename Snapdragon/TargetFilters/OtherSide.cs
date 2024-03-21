﻿namespace Snapdragon.TargetFilters
{
    public record OtherSide : ICardFilter<ICard>, ISideFilter<ICard>, ISideFilter<CardEvent>
    {
        public bool Applies(ICard card, ICard source, Game game)
        {
            return (card.Side.Other() == source.Side);
        }

        public bool Applies(Side side, ICard source, Game game)
        {
            return side.Other() == source.Side;
        }

        public bool Applies(Side side, CardEvent source, Game game)
        {
            return side.Other() == source.Card.Side;
        }
    }
}
