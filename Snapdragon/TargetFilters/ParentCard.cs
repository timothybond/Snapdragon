namespace Snapdragon.TargetFilters
{
    public class ParentCard(Sensor<CardInstance> Sensor) : ICardFilter
    {
        public bool Applies(ICard card, Game game)
        {
            return (card.Id == Sensor.Source.Id);
        }
    }
}
