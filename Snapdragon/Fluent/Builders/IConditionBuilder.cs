namespace Snapdragon.Fluent.Builders
{
    public interface IConditionBuilder<TResult, TContext>
    {
        IBuilderWithCondition<TResult, TContext> WithCondition(ICondition<TContext> condition);
    }
}
