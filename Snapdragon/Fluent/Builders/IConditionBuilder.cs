namespace Snapdragon.Fluent.Builders
{
    public interface IConditionBuilder<TAbility, TContext, TOutcome>
    {
        IBuilderWithCondition<TAbility, TContext, TOutcome> WithCondition(
            ICondition<TContext> condition
        );
    }
}
