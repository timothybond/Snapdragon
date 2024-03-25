namespace Snapdragon.Fluent.Builders
{
    public record Builder<TAbility, TContext, TOutcome>(
        IResultFactory<TAbility, TContext, TOutcome> Factory
    ) : IBuilder<TAbility, TContext, TOutcome>
    {
        public ConditionBuilder<TAbility, TContext, TOutcome> If
        {
            get { return new ConditionBuilder<TAbility, TContext, TOutcome>(Factory); }
        }

        public virtual TAbility Build(TOutcome outcome)
        {
            return Factory.Build(outcome);
        }
    }

    public record Builder<TAbility, TEvent, TContext, TOutcome>(
        IResultFactory<TAbility, TEvent, TContext, TOutcome> Factory
    ) : IBuilder<TAbility, TContext, TOutcome>
        where TEvent : Event
    {
        public virtual IConditionBuilder<TAbility, TEvent, TContext, TOutcome> If
        {
            get { return new ConditionBuilder<TAbility, TEvent, TContext, TOutcome>(Factory); }
        }

        public BuilderWithEventFilter<TAbility, TEvent, TContext, TOutcome> Where(
            IEventFilter<TEvent, TContext> eventFilter
        )
        {
            return new BuilderWithEventFilter<TAbility, TEvent, TContext, TOutcome>(
                Factory,
                eventFilter
            );
        }

        public virtual TAbility Build(TOutcome outcome)
        {
            return Factory.Build(outcome);
        }
    }
}
