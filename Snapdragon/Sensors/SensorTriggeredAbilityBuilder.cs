namespace Snapdragon.Sensors
{
    public record SensorTriggeredAbilityBuilder(
        ITriggerBuilder<Sensor<Card>> TriggerBuilder,
        IEffectBuilder<Sensor<Card>> EffectBuilder,
        bool DeleteOnActivation = true
    ) : ITriggeredSensorAbilityBuilder<Sensor<Card>>
    {
        public TriggeredSensorAbility<Sensor<Card>> Build(Game game, Sensor<Card> source)
        {
            if (DeleteOnActivation)
            {
                return new TriggeredSensorAbility<Sensor<Card>>(
                    TriggerBuilder.Build(game, source),
                    new DeleteOnTrigger(EffectBuilder).Build(game, source)
                );
            }
            else
            {
                return new TriggeredSensorAbility<Sensor<Card>>(
                    TriggerBuilder.Build(game, source),
                    EffectBuilder.Build(game, source)
                );
            }
        }
    }
}
