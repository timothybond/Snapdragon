namespace Snapdragon.Fluent.Builders
{
    public record RevealBuilderWithCondition<TContext>(ICondition<TContext> Condition)
        : RevealBuilder<TContext>,
            IBuilderWithCondition<OnReveal<TContext>, TContext>
    {
        public override OnReveal<TContext> Build(IEffectBuilder<TContext> effectBuilder)
        {
            return new OnReveal<TContext>(effectBuilder, Condition);
        }

        public IConditionBuilder<OnReveal<TContext>, TContext> And
        {
            get { return new RevealChainedConditionBuilder<TContext>(Condition); }
        }
    }
}
