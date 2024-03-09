using Snapdragon.Events;
using Snapdragon.Triggers;

namespace Snapdragon.Sensors
{
    public record CardPlayed : ITriggerBuilder<Sensor<CardInstance>, CardPlayedEvent>
    {
        public ITrigger<CardPlayedEvent> Build(Game game, Sensor<CardInstance> source)
        {
            return new OnPlayCard(null, source.Side, null);
        }
    }
}
