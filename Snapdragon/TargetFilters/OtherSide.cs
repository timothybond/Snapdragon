namespace Snapdragon.TargetFilters
{
    public record OtherSide : ICardFilter<Card>
    {
        public bool Applies(Card card, Card source, Game game)
        {
            return (card.Side.OtherSide() == source.Side);
        }
    }
}
