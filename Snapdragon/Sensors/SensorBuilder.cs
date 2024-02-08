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
                null,
                MoveAbilityBuilder?.Build(game, source)
            );

            var ability = AbilityBuilder.Build(game, sensor);
            sensor = sensor with { TriggeredAbility = ability };

            return sensor;
        }
    }
}
