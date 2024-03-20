using Snapdragon.Fluent;

namespace Snapdragon.Effects
{
    public record CreateTriggeredSensor<TEvent>(
        Card Source,
        TriggeredAbility<TEvent, Sensor<Card>> SensorAbility
    ) : IEffect
        where TEvent : Event
    {
        public Game Apply(Game game)
        {
            // TODO: Get rid of duplication with CreateSensor reveal ability

            var sensor = new Sensor<Card>(
                Ids.GetNext<Sensor<Card>>(),
                Source.Column,
                Source.Side,
                Source,
                null // TODO - Enable ability
            );

            return game.WithSensor(sensor);
        }
    }
}
