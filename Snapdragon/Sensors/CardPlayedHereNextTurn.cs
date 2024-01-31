using Snapdragon.Triggers;

namespace Snapdragon.Sensors
{
    public record CardPlayedHereNextTurn : ITriggerBuilder<Sensor<Card>>
    {
        public ITrigger Build(Game game, Sensor<Card> source)
        {
            return new OnPlayCard(source.Column, source.Side, game.Turn + 1);
        }
    }
}
