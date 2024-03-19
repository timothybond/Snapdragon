namespace Snapdragon.Fluent.Builders
{
    public record PastEventConditionBuilder<TAbility, TContext, TOutcome>(
        IResultFactory<TAbility, TContext, TOutcome> Factory
    )
    {
        public PastEventOfTypeConditionBuilder<
            TEvent,
            TAbility,
            TContext,
            TOutcome
        > OfType<TEvent>()
        {
            return new PastEventOfTypeConditionBuilder<TEvent, TAbility, TContext, TOutcome>(
                Factory
            );
        }
    }
}
