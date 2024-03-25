namespace Snapdragon.Fluent.Builders
{
    public record EventFilteredConditionBuilder<TAbility, TEvent, TContext, TOutcome>(
        IResultFactory<TAbility, TEvent, TContext, TOutcome> Factory,
        IEventFilter<TEvent, TContext> EventFilter
    ) : IConditionBuilder<TAbility, TEvent, TContext, TOutcome>
        where TEvent : Event
    {
        IBuilderWithCondition<TAbility, TEvent, TContext, TOutcome> IConditionBuilder<
            TAbility,
            TEvent,
            TContext,
            TOutcome
        >.WithCondition(ICondition<TEvent, TContext> condition)
        {
            return new BuilderWithEventFilterAndCondition<TAbility, TEvent, TContext, TOutcome>(
                Factory,
                EventFilter,
                condition
            );
        }
    }
}
