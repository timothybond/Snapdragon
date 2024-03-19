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
}
