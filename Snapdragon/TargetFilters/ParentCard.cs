namespace Snapdragon.TargetFilters
{
    public class ParentCard(Sensor<Card> TemporaryEffect) : ICardFilter
    {
        public bool Applies(Card card, Game game)
        {
            return (card.Id == TemporaryEffect.Source.Id);
        }
    }
}
