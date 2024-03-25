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

    public record NoPastEventConditionBuilder<TAbility, TEvent, TContext, TOutcome>(
        IConditionBuilder<TAbility, TEvent, TContext, TOutcome> PriorBuilder
    )
        where TEvent : Event
    {
        public NoPastEventOfTypeConditionBuilder<
            TPastEvent,
            TAbility,
            TEvent,
            TContext,
            TOutcome
        > OfType<TPastEvent>()
            where TPastEvent : Event
        {
            return new NoPastEventOfTypeConditionBuilder<
                TPastEvent,
                TAbility,
                TEvent,
                TContext,
                TOutcome
            >(PriorBuilder);
        }
    }
}
