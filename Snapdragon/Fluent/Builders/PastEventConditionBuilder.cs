namespace Snapdragon.Fluent.Builders
{
    public record PastEventConditionBuilder<TResult, TContext>(
        IConditionBuilder<TResult, TContext> ConditionBuilder
    )
    {
        public PastEventOfTypeConditionBuilder<TEvent, TResult, TContext> OfType<TEvent>()
        {
            return new PastEventOfTypeConditionBuilder<TEvent, TResult, TContext>(ConditionBuilder);
        }
    }
}
