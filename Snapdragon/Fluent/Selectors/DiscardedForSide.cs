namespace Snapdragon.Fluent.Selectors
{
    public record DiscardedForSide(bool OtherSide) : ISelector<ICardInstance, IObjectWithSide>
    {
        public IEnumerable<ICardInstance> Get(IObjectWithSide context, Game game)
        {
            var side = OtherSide ? context.Side.Other() : context.Side;
            return game[side].Discards;
        }

        public bool Selects(ICardInstance item, IObjectWithSide context, Game game)
        {
            return item.Side == context.Side && item.State == CardState.Discarded;
        }
    }
}
