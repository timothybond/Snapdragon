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
            where TEvent : Event
        {
            return new PastEventOfTypeConditionBuilder<TEvent, TAbility, TContext, TOutcome>(
                PriorBuilder
            );
        }
    }

    public record PastEventConditionBuilder<TAbility, TEvent, TContext, TOutcome>(
        IConditionBuilder<TAbility, TEvent, TContext, TOutcome> PriorBuilder
    )
        where TEvent : Event
    {
        public PastEventOfTypeConditionBuilder<
            TPastEvent,
            TAbility,
            TEvent,
            TContext,
            TOutcome
        > OfType<TPastEvent>()
            where TPastEvent : Event
        {
            return new PastEventOfTypeConditionBuilder<
                TPastEvent,
                TAbility,
                TEvent,
                TContext,
                TOutcome
            >(PriorBuilder);
        }
    }
}
