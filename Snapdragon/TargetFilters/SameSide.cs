namespace Snapdragon.TargetFilters
{
    public record SameSide<T> : ICardFilter<T>, ISideFilter<T>
        where T : ICard
    {
        public bool Applies(ICard card, T source, Game game)
        {
            return (card.Side == source.Side);
        }

        public bool Applies(Side side, T source, Game game)
        {
            return side == source.Side;
        }
    }
}
