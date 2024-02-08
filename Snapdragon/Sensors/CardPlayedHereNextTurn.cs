using Snapdragon.Events;
using Snapdragon.Triggers;

namespace Snapdragon.Sensors
{
    public record CardPlayedHereNextTurn : ITriggerBuilder<Sensor<Card>, CardPlayedEvent>
    {
        public ITrigger<CardPlayedEvent> Build(Game game, Sensor<Card> source)
        {
            return new OnPlayCard(source.Column, source.Side, game.Turn + 1);
        }
    }
}
