namespace Snapdragon.Fluent.Builders
{
    public record PastEventConditionBuilder<TAbility, TContext, TOutcome>(
        IConditionBuilder<TAbility, TContext, TOutcome> PriorBuilder
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
                PriorBuilder
            );
        }
    }
}
