namespace Snapdragon.TemporaryEffects
{
    public record TemporaryEffectTriggeredAbilityBuilder(
        ITriggerBuilder<TemporaryEffect<Card>> TriggerBuilder,
        IEffectBuilder<TemporaryEffect<Card>> EffectBuilder,
        bool DeleteOnActivation = true
    ) : ITriggeredAbilityBuilder<TemporaryEffect<Card>>
    {
        public TriggeredAbility<TemporaryEffect<Card>> Build(
            Game game,
            TemporaryEffect<Card> source
        )
        {
            if (DeleteOnActivation)
            {
                return new TriggeredAbility<TemporaryEffect<Card>>(
                    TriggerBuilder.Build(game, source),
                    new DeleteOnTrigger(EffectBuilder).Build(game, source)
                );
            }
            else
            {
                return new TriggeredAbility<TemporaryEffect<Card>>(
                    TriggerBuilder.Build(game, source),
                    EffectBuilder.Build(game, source)
                );
            }
        }
    }
}
