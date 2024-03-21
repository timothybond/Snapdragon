namespace Snapdragon.Fluent.Builders
{
    public record BuilderWithCondition<TAbility, TContext, TOutcome>(
        ICondition<TContext> Condition,
        IResultFactory<TAbility, TContext, TOutcome> Factory
    )
        : Builder<TAbility, TContext, TOutcome>(Factory),
            IBuilderWithCondition<TAbility, TContext, TOutcome>
    {
        public override TAbility Build(TOutcome outcome)
        {
            return Factory.Build(outcome, Condition);
        }

        public IConditionBuilder<TAbility, TContext, TOutcome> And
        {
            get
            {
                return new ChainedConditionBuilder<TAbility, TContext, TOutcome>(
                    Condition,
                    Factory
                );
            }
        }
    }

    public record BuilderWithCondition<TAbility, TEvent, TContext, TOutcome>(
        ICondition<TEvent, TContext> Condition,
        IResultFactory<TAbility, TEvent, TContext, TOutcome> Factory
    )
        : Builder<TAbility, TEvent, TContext, TOutcome>(Factory),
            IBuilderWithCondition<TAbility, TEvent, TContext, TOutcome> where TEvent : Event
    {
        public override TAbility Build(TOutcome outcome)
        {
            return Factory.Build(outcome, null, Condition);
        }

        public IConditionBuilder<TAbility, TEvent, TContext, TOutcome> And
        {
            get
            {
                return new ChainedConditionBuilder<TAbility, TEvent, TContext, TOutcome>(
                    Condition,
                    Factory
                );
            }
        }
    }
}
