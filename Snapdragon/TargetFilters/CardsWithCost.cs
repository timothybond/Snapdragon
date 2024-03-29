namespace Snapdragon.TargetFilters
{
    public record CardsWithCost(int Cost) : ICardFilter<object>
    {
        public bool Applies(ICardInstance card, object source, Game game)
        {
            return card.Cost == this.Cost;
        }
    }
}
