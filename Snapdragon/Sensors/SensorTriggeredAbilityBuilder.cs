namespace Snapdragon.Sensors
{
    public record SensorTriggeredAbilityBuilder(
        ITriggerBuilder<Sensor<Card>> TriggerBuilder,
        ISourceTriggeredEffectBuilder<Sensor<Card>> EffectBuilder,
        bool DeleteOnActivation = true
    ) : ISensorTriggeredAbilityBuilder<Sensor<Card>>
    {
        public ISensorTriggeredAbility Build(Game game, Sensor<Card> source)
        {
            if (DeleteOnActivation)
            {
                return new TriggeredSensorAbility(
                    TriggerBuilder.Build(game, source),
                    new DeleteOnTrigger(EffectBuilder)
                );
            }
            else
            {
                return new TriggeredSensorAbility(
                    TriggerBuilder.Build(game, source),
                    EffectBuilder
                );
            }
        }
    }
}
