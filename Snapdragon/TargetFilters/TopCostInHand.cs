namespace Snapdragon.TargetFilters
{
    public record TopCostInHand<T> : ICardFilter<T>
    {
        public bool Applies(Card card, T source, Game game)
        {
            if (card.State != CardState.InHand)
            {
                return false;
            }

            var hand = game[card.Side].Hand;

            if (hand.IsEmpty)
            {
                throw new InvalidOperationException("Card state was InHand but Hand is empty.");
            }

            var topCost = hand.Max(c => c.Cost);

            return card.Cost == topCost;
        }
    }
}
