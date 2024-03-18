namespace Snapdragon.Fluent.Builders
{
    public interface IBuilderWithCondition<TResult, TContext> : IBuilder<TResult, TContext>
    {
        public IConditionBuilder<TResult, TContext> And { get; }
    }
}
