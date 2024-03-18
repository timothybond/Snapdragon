namespace Snapdragon.Fluent.Builders
{
    public record RevealChainedConditionBuilder<TContext>(ICondition<TContext> Condition)
        : RevealConditionBuilder<TContext>
    {
        public RevealBuilderWithCondition<TContext> WithCondition(ICondition<TContext> newCondition)
        {
            return new RevealBuilderWithCondition<TContext>(
                new AndCondition<TContext>(Condition, newCondition)
            );
        }
    }
}
