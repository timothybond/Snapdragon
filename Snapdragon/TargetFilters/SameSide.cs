namespace Snapdragon.TargetFilters
{
    public record SameSide : ICardFilter<ICardInstance>, ISideFilter<ICardInstance>, ISideFilter<CardEvent>
    {
        public bool Applies(ICardInstance card, ICardInstance source, Game game)
        {
            return (card.Side == source.Side);
        }

        public bool Applies(Side side, ICardInstance source, Game game)
        {
            return side == source.Side;
        }

        public bool Applies(Side side, CardEvent source, Game game)
        {
            return side == source.Card.Side;
        }
    }
}
