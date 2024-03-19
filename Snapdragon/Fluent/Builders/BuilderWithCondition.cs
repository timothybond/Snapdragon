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
}
