namespace Snapdragon.Fluent.Builders
{
    public record ChainedEventFilteredConditionBuilder<TAbility, TEvent, TContext, TOutcome>(
        IResultFactory<TAbility, TEvent, TContext, TOutcome> Factory,
        IEventFilter<TEvent, TContext> EventFilter,
        ICondition<TEvent, TContext> Condition
    ) : ConditionBuilder<TAbility, TEvent, TContext, TOutcome>(Factory)
        where TEvent : Event
    {
        public BuilderWithEventFilterAndCondition<
            TAbility,
            TEvent,
            TContext,
            TOutcome
        > WithCondition(ICondition<TEvent, TContext> newCondition)
        {
            return new BuilderWithEventFilterAndCondition<TAbility, TEvent, TContext, TOutcome>(
                Factory,
                EventFilter,
                new AndCondition<TEvent, TContext>(Condition, newCondition)
            );
        }
    }
}
