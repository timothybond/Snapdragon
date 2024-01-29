namespace Snapdragon.TargetFilters
{
    public class ParentCard(TemporaryEffect<Card> TemporaryEffect) : ICardFilter
    {
        public bool Applies(Card card)
        {
            return (card.Id == TemporaryEffect.Source.Id);
        }
    }
}
