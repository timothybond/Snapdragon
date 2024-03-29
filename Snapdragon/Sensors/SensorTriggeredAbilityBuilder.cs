namespace Snapdragon.Sensors
{
    public record SensorTriggeredAbilityBuilder<TEvent>(
        ITriggerBuilder<Sensor<ICard>, TEvent> TriggerBuilder,
        ISourceTriggeredEffectBuilder<Sensor<ICard>, TEvent> EffectBuilder,
        bool DeleteOnActivation = true
    ) : ISensorTriggeredAbilityBuilder<Sensor<ICard>, TEvent>
        where TEvent : Event
    {
        public ITriggeredAbility<Sensor<ICard>> Build(Game game, Sensor<ICard> source)
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
