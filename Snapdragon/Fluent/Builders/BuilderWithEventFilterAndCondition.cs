namespace Snapdragon.Fluent.Builders
{
    public record BuilderWithEventFilterAndCondition<TAbility, TEvent, TContext, TOutcome>(
        IResultFactory<TAbility, TEvent, TContext, TOutcome> Factory,
        IEventFilter<TEvent, TContext> EventFilter,
        ICondition<TEvent, TContext> Condition
    ) : IBuilderWithCondition<TAbility, TEvent, TContext, TOutcome>
        where TEvent : Event
    {
        public IConditionBuilder<TAbility, TEvent, TContext, TOutcome> And
        {
            get
            {
                return new ChainedEventFilteredConditionBuilder<
                    TAbility,
                    TEvent,
                    TContext,
                    TOutcome
                >(Factory, EventFilter, Condition);
            }
        }

        public TAbility Then(TOutcome outcome)
        {
            return Factory.Build(outcome, EventFilter, Condition);
        }
    }
}
