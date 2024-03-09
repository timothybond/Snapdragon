namespace Snapdragon.Sensors
{
    public record SensorBuilder<TEvent>(
        ISensorTriggeredAbilityBuilder<Sensor<Card>, TEvent> AbilityBuilder,
        ISensorMoveAbilityBuilder<Card>? MoveAbilityBuilder = null
    )
        where TEvent : Event
    {
        public Sensor<Card> Build(Game game, Card source)
        {
            var sensor = new Sensor<Card>(
                Ids.GetNext<Sensor<Card>>(),
                source.Column,
                source.Side,
                source,
                null,
                MoveAbilityBuilder?.Build(game, source)
            );

            var ability = AbilityBuilder.Build(game, sensor);
            sensor = sensor with { TriggeredAbility = ability };

            return sensor;
        }
    }
}
