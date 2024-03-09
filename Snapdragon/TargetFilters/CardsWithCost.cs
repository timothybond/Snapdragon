namespace Snapdragon.TargetFilters
{
    public record CardsWithCost<T>(int Cost) : ICardFilter<T> where T : ICard
    {
        public bool Applies(ICard card, T source, Game game)
        {
            return card.Cost == this.Cost;
        }
    }
}
