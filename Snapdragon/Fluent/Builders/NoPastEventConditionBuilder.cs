namespace Snapdragon.Fluent.Builders
{
    public record NoPastEventConditionBuilder<TAbility, TContext, TOutcome>(
        IConditionBuilder<TAbility, TContext, TOutcome> PriorBuilder
    )
    {
        public NoPastEventOfTypeConditionBuilder<
            TEvent,
            TAbility,
            TContext,
            TOutcome
        > OfType<TEvent>()
            where TEvent : Event
        {
            return new NoPastEventOfTypeConditionBuilder<TEvent, TAbility, TContext, TOutcome>(
                PriorBuilder
            );
        }
    }
}
