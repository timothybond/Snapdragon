using Snapdragon.Triggers;

namespace Snapdragon.Sensors
{
    public record CardRevealed : ITriggerBuilder<Sensor<Card>>
    {
        public ITrigger Build(Game game, Sensor<Card> source)
        {
            return new OnRevealCard(null, source.Side, null, source.Source);
        }
    }
}
