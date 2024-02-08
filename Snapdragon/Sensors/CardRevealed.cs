using Snapdragon.Events;
using Snapdragon.Triggers;

namespace Snapdragon.Sensors
{
    public record CardRevealed : ITriggerBuilder<Sensor<Card>, CardRevealedEvent>
    {
        public ITrigger<CardRevealedEvent> Build(Game game, Sensor<Card> source)
        {
            return new OnRevealCard(null, source.Side, null, source.Source);
        }
    }
}
