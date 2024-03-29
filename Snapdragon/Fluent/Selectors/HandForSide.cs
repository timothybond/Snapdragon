﻿namespace Snapdragon.Fluent.Selectors
{
    public record HandForSide(bool OtherSide) : ISelector<ICardInstance, IObjectWithSide>
    {
        public IEnumerable<ICardInstance> Get(IObjectWithSide context, Game game)
        {
            var side = OtherSide ? context.Side.Other() : context.Side;
            return game[side].Hand;
        }
    }
}
