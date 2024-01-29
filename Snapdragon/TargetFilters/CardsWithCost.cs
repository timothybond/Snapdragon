namespace Snapdragon.TargetFilters
{
    public record CardsWithCost(int Cost) : ICardFilter<Card>
    {
        public bool Applies(Card card, Card source)
        {
            return card.Cost == this.Cost;
        }
    }
}
