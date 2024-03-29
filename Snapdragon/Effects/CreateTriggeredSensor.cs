namespace Snapdragon.Effects
{
    public record CreateTriggeredSensor(
        ICard Source,
        ITriggeredAbility<Sensor<ICard>> TriggeredAbility
    ) : IEffect
    {
        public Game Apply(Game game)
        {
            // TODO: Get rid of duplication with CreateSensor reveal ability

            var sensor = new Sensor<ICard>(
                Ids.GetNextSensor(),
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
