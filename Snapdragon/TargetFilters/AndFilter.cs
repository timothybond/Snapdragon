namespace Snapdragon.TargetFilters
{
    public record AndFilter<T>(ICardFilter<T> First, ICardFilter<T> Second) : ICardFilter<T>
    {
        public bool Applies(ICardInstance card, T source, Game game)
        {
            return First.Applies(card, source, game) && Second.Applies(card, source, game);
        }
    }
}
