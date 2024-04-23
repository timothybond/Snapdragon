namespace Snapdragon.Fluent.Builders
{
    public record BuilderWithEventFilter<TAbility, TEvent, TContext, TOutcome>(
        IResultFactory<TAbility, TEvent, TContext, TOutcome> Factory,
        IEventFilter<TEvent, TContext> EventFilter
    )
        : Builder<TAbility, TEvent, TContext, TOutcome>(Factory),
            IBuilder<TAbility, TContext, TOutcome>
        where TEvent : Event
    {
        public override TAbility Then(TOutcome outcome)
        {
            return Factory.Build(outcome, EventFilter);
        }

        public override IConditionBuilder<TAbility, TEvent, TContext, TOutcome> If
        {
            get
            {
                return new EventFilteredConditionBuilder<TAbility, TEvent, TContext, TOutcome>(
                    Factory,
                    EventFilter
                );
            }
        }
    }
}
