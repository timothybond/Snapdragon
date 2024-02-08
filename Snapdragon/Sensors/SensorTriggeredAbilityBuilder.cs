namespace Snapdragon.Sensors
{
    public record SensorTriggeredAbilityBuilder<TEvent>(
        ITriggerBuilder<Sensor<Card>, TEvent> TriggerBuilder,
        ISourceTriggeredEffectBuilder<Sensor<Card>, TEvent> EffectBuilder,
        bool DeleteOnActivation = true
    ) : ISensorTriggeredAbilityBuilder<Sensor<Card>, TEvent>
        where TEvent : Event
    {
        public ISensorTriggeredAbility Build(Game game, Sensor<Card> source)
        {
            if (DeleteOnActivation)
            {
                return new TriggeredSensorAbility<TEvent>(
                    TriggerBuilder.Build(game, source),
                    new DeleteOnTrigger<TEvent>(EffectBuilder)
                );
            }
            else
            {
                return new TriggeredSensorAbility<TEvent>(
                    TriggerBuilder.Build(game, source),
                    EffectBuilder
                );
            }
        }
    }
}
