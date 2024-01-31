namespace Snapdragon.Sensors
{
    public record SensorBuilder(SensorTriggeredAbilityBuilder AbilityBuilder)
    {
        public Sensor<Card> Build(Game game, Card source)
        {
            var column =
                source.Column
                ?? throw new InvalidOperationException(
                    "Attempted to build a temporary effect from a card that isn't in play."
                );

            var sensor = new Sensor<Card>(
                Ids.GetNext<Sensor<Card>>(),
                column,
                source.Side,
                source,
                null
            );

            var ability = AbilityBuilder.Build(game, sensor);
            sensor = sensor with { Ability = ability };

            return sensor;
        }
    }
}
