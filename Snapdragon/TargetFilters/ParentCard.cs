namespace Snapdragon.TargetFilters
{
    public class ParentCard(TemporaryEffect<Card> TemporaryEffect) : ICardFilter
    {
        public bool Applies(Card card, GameState game)
        {
            return (card.Id == TemporaryEffect.Source.Id);
        }
    }
}
