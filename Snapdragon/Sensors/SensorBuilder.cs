namespace Snapdragon.Sensors
{
    public record SensorBuilder<TEvent>(
        ISensorTriggeredAbilityBuilder<Sensor<ICard>, TEvent> AbilityBuilder,
        ISensorMoveAbilityBuilder<ICard>? MoveAbilityBuilder = null
    )
        where TEvent : Event
    {
        public Sensor<ICard> Build(Game game, ICard source)
        {
            var sensor = new Sensor<ICard>(
                Ids.GetNextSensor(),
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
