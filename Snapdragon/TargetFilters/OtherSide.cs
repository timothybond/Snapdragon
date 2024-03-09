namespace Snapdragon.TargetFilters
{
    public record OtherSide : ICardFilter<Card>
    {
        public bool Applies(ICard card, Card source, Game game)
        {
            return (card.Side.Other() == source.Side);
        }
    }
}
