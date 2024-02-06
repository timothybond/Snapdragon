using Snapdragon.Triggers;

namespace Snapdragon.Sensors
{
    public record CardPlayed : ITriggerBuilder<Sensor<Card>>
    {
        public ITrigger Build(Game game, Sensor<Card> source)
        {
            return new OnPlayCard(null, source.Side, null);
        }
    }
}
