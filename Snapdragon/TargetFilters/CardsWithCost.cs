namespace Snapdragon.TargetFilters
{
    public record CardsWithCost(int Cost) : ICardFilter<Card>
    {
        public bool Applies(Card card, Card source, GameState game)
        {
            return card.Cost == this.Cost;
        }
    }
}
