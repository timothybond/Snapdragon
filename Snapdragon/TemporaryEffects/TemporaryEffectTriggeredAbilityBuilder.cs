namespace Snapdragon.TemporaryEffects
{
    public record TemporaryEffectTriggeredAbilityBuilder(
        ITriggerBuilder<TemporaryEffect<Card>> TriggerBuilder,
        IEffectBuilder<TemporaryEffect<Card>> EffectBuilder,
        bool DeleteOnActivation = true
    ) : ITriggeredAbilityBuilder<TemporaryEffect<Card>>
    {
        public TriggeredEffectAbility<TemporaryEffect<Card>> Build(
            Game game,
            TemporaryEffect<Card> source
        )
        {
            if (DeleteOnActivation)
            {
                return new TriggeredEffectAbility<TemporaryEffect<Card>>(
                    TriggerBuilder.Build(game, source),
                    new DeleteOnTrigger(EffectBuilder).Build(game, source)
                );
            }
            else
            {
                return new TriggeredEffectAbility<TemporaryEffect<Card>>(
                    TriggerBuilder.Build(game, source),
                    EffectBuilder.Build(game, source)
                );
            }
        }
    }
}
