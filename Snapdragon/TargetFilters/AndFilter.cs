namespace Snapdragon.TargetFilters
{
    public record AndFilter(ICardFilter First, ICardFilter Second) : ICardFilter
    {
        public bool Applies(Card card, Card source)
        {
            return First.Applies(card, source) && Second.Applies(card, source);
        }
    }
}
