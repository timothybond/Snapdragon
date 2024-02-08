using Snapdragon.Events;
using Snapdragon.Triggers;

namespace Snapdragon.Sensors
{
    public record CardPlayed : ITriggerBuilder<Sensor<Card>, CardPlayedEvent>
    {
        public ITrigger<CardPlayedEvent> Build(Game game, Sensor<Card> source)
        {
            return new OnPlayCard(null, source.Side, null);
        }
    }
}
