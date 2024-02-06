namespace Snapdragon.Sensors
{
    public record SensorTriggeredAbilityBuilder(
        ITriggerBuilder<Sensor<Card>> TriggerBuilder,
        IEffectBuilder<Sensor<Card>> EffectBuilder,
        bool DeleteOnActivation = true
    ) : ISensorTriggeredAbilityBuilder<Sensor<Card>>
    {
        public ISensorTriggeredAbility Build(Game game, Sensor<Card> source)
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
