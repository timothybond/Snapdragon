namespace Snapdragon.Fluent.Builders
{
    public record BuilderWithEventFilter<TAbility, TEvent, TContext, TOutcome>(
        IResultFactory<TAbility, TEvent, TContext, TOutcome> Factory,
        IEventFilter<TEvent, TContext> EventFilter,
        ICondition<TContext>? Condition = null
    )
        : Builder<TAbility, TEvent, TContext, TOutcome>(Factory),
            IBuilder<TAbility, TContext, TOutcome>
    {
        public override TAbility Build(TOutcome outcome)
        {
            return Factory.Build(outcome, EventFilter, Condition);
        }
    }
}
