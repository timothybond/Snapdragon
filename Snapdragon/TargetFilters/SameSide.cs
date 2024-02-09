namespace Snapdragon.TargetFilters
{
    public record SameSide : ICardFilter<Card>, ISideFilter<Card>
    {
        public bool Applies(Card card, Card source, Game game)
        {
            return (card.Side == source.Side);
        }

        public bool Applies(Side side, Card source, Game game)
        {
            return side == source.Side;
        }
    }
}
