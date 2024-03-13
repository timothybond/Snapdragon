namespace Snapdragon.TargetFilters
{
    public record RightmostCardInHand : ICardFilter<object>
    {
        public bool Applies(ICard card, object source, Game game)
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

            return hand.Last().Id == card.Id;
        }
    }
}
