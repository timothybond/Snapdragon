namespace Snapdragon.Sensors
{
    /// <summary>
    /// Builds a <see cref="Sensor{Card}"/> that also self-destructs after the number of turns has elapsed.
    /// </summary>
    /// <param name="Turns">The number of additional turns after which the temporary effect is deleted. 0 for "this turn".</param>
    /// <param name="TriggerBuilder">Builds the trigger for the ability for the <see cref="Sensor{Card}/></param>
    /// <param name="EffectBuilder">Builds the main effect to be potentially triggered.</param>
    public class ExpiringTriggeredSensorBuilder<TEvent>(
        int Turns,
        ITriggerBuilder<Sensor<Card>, TEvent>? TriggerBuilder = null,
        ISourceTriggeredEffectBuilder<Sensor<Card>, TEvent>? EffectBuilder = null
    ) : ISensorTriggeredAbilityBuilder<Sensor<Card>, TEvent>
        where TEvent : Event
    {
        public ITriggeredAbility<Sensor<Card>> Build(Game game, Sensor<Card> source)
        {
            var expiresAtTurn = game.Turn + Turns;

            if (TriggerBuilder == null && EffectBuilder == null)
            {
                return new ExpiringSensorTriggeredAbility<TEvent>(expiresAtTurn, source, null);
            }

            if (TriggerBuilder == null || EffectBuilder == null)
            {
                throw new InvalidOperationException(
                    "Created an ExpiringTriggeredSensorBuilder with a null TriggerBuilder or EffectBuilder, but not both."
                );
            }

            var innerTrigger = TriggerBuilder.Build(game, source);

            return new ExpiringSensorTriggeredAbility<TEvent>(
                expiresAtTurn,
                source,
                new TriggeredSensorAbility<TEvent>(innerTrigger, EffectBuilder)
            );
        }
    }
}
