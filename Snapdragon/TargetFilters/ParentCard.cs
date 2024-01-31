namespace Snapdragon.TargetFilters
{
    public class ParentCard(Sensor<Card> Sensor) : ICardFilter
    {
        public bool Applies(Card card, Game game)
        {
            return (card.Id == Sensor.Source.Id);
        }
    }
}
