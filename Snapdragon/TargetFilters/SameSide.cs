namespace Snapdragon.TargetFilters
{
    public record SameSide : ICardFilter
    {
        public bool Applies(Card card, Card source)
        {
            return (card.Side == source.Side);
        }
    }
}
