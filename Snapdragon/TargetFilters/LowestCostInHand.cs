namespace Snapdragon.TargetFilters
{
    public record LowestCostInHand : ICardFilter<object>
    {
        public bool Applies(ICardInstance card, object source, Game game)
        {
            if (card.State != CardState.InHand)
            {
                return false;
            }

            var hand = game[card.Side].Hand;

            if (hand.Count == 0)
            {
                throw new InvalidOperationException("Card state was InHand but Hand is empty.");
            }

            var lowestCost = hand.Min(c => c.Cost);

            return card.Cost == lowestCost;
        }
    }
}
