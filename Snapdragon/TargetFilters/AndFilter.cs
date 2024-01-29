namespace Snapdragon.TargetFilters
{
    public record AndFilter<T>(ICardFilter<T> First, ICardFilter<T> Second) : ICardFilter<T>
    {
        public bool Applies(Card card, T source)
        {
            return First.Applies(card, source) && Second.Applies(card, source);
        }
    }
}
