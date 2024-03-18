namespace Snapdragon.Fluent.Builders
{
    public record RevealConditionBuilder<TContext>()
        : IConditionBuilder<OnReveal<TContext>, TContext>
    {
        IBuilderWithCondition<OnReveal<TContext>, TContext> IConditionBuilder<
            OnReveal<TContext>,
            TContext
        >.WithCondition(ICondition<TContext> condition)
        {
            return new RevealBuilderWithCondition<TContext>(condition);
        }

        public PastEventConditionBuilder<OnReveal<TContext>, TContext> PastEvent
        {
            get { return new PastEventConditionBuilder<OnReveal<TContext>, TContext>(this); }
        }
    }
}
