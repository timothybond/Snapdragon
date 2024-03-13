namespace Snapdragon.TargetFilters
{
    public record SameSide : ICardFilter<ICard>, ISideFilter<ICard>, ISideFilter<ICardEvent>
    {
        public bool Applies(ICard card, ICard source, Game game)
        {
            return (card.Side == source.Side);
        }

        public bool Applies(Side side, ICard source, Game game)
        {
            return side == source.Side;
        }

        public bool Applies(Side side, ICardEvent source, Game game)
        {
            return side == source.Card.Side;
        }
    }
}
