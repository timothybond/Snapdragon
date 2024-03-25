namespace Snapdragon.Effects
{
    public record CreateTriggeredSensor(
        Card Source,
        ITriggeredAbility<Sensor<Card>> TriggeredAbility
    ) : IEffect
    {
        public Game Apply(Game game)
        {
            // TODO: Get rid of duplication with CreateSensor reveal ability

            var sensor = new Sensor<Card>(
                Ids.GetNext<Sensor<Card>>(),
                Source.Column,
                Source.Side,
                Source,
                TriggeredAbility,
                null,
                game.Turn
            );

            return game.WithSensor(sensor);
        }
    }
}
