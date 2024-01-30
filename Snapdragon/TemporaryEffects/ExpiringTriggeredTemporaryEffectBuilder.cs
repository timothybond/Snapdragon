namespace Snapdragon.TemporaryEffects
{
    /// <summary>
    /// Builds a <see cref="TemporaryEffect{Card}"/> that also self-destructs after the number of turns has elapsed.
    /// </summary>
    /// <param name="Turns">The number of additional turns after which the temporary effect is deleted. 0 for "this turn".</param>
    /// <param name="TriggerBuilder">Builds the trigger for the ability for the <see cref="TemporaryEffect{Card}/></param>
    /// <param name="EffectBuilder">Builds the main effect to be potentially triggered.</param>
    public class ExpiringTriggeredTemporaryEffectBuilder(
        int Turns,
        ITriggerBuilder<TemporaryEffect<Card>> TriggerBuilder,
        IEffectBuilder<TemporaryEffect<Card>> EffectBuilder) : ITriggeredAbilityBuilder<TemporaryEffect<Card>>
    {
        public TriggeredAbility<TemporaryEffect<Card>> Build(Game game, TemporaryEffect<Card> source)
        {
            var innerTrigger = TriggerBuilder.Build(game, source);
            var innerEffect = EffectBuilder.Build(game, source);

            throw new NotImplementedException();
        }
    }
}
