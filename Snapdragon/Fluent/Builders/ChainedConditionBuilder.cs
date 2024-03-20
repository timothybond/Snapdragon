namespace Snapdragon.Fluent.Builders
{
    public record ChainedConditionBuilder<TAbility, TContext, TOutcome>(
        ICondition<TContext> Condition,
        IResultFactory<TAbility, TContext, TOutcome> Factory
    ) : ConditionBuilder<TAbility, TContext, TOutcome>(Factory)
    {
        public BuilderWithCondition<TAbility, TContext, TOutcome> WithCondition(
            ICondition<TContext> newCondition
        )
        {
            return new BuilderWithCondition<TAbility, TContext, TOutcome>(
                new AndCondition<TContext>(Condition, newCondition),
                Factory
            );
        }
    }

    public record ChainedConditionBuilder<TAbility, TEvent, TContext, TOutcome>(
        ICondition<TContext> Condition,
        IResultFactory<TAbility, TEvent, TContext, TOutcome> Factory
    ) : ConditionBuilder<TAbility, TEvent, TContext, TOutcome>(Factory)
    {
        public BuilderWithCondition<TAbility, TEvent, TContext, TOutcome> WithCondition(
            ICondition<TContext> newCondition
        )
        {
            return new BuilderWithCondition<TAbility, TEvent, TContext, TOutcome>(
                new AndCondition<TContext>(Condition, newCondition),
                Factory
            );
        }
    }
}
