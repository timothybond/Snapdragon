namespace Snapdragon.Fluent.Selectors
{
    public record DiscardedForSide(bool OtherSide) : ICardSelector<ICard>
    {
        public IEnumerable<ICard> Get(ICard context, Game game)
        {
            var side = OtherSide ? context.Side.Other() : context.Side;
            return game[side].Discards;
        }
    }
}
