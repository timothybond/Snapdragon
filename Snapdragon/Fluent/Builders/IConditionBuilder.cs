namespace Snapdragon.Fluent.Builders
{
    public interface IConditionBuilder<TAbility, TContext, TOutcome>
    {
        IBuilderWithCondition<TAbility, TContext, TOutcome> WithCondition(
            ICondition<TContext> condition
        );
    }

    public interface IConditionBuilder<TAbility, TEvent, TContext, TOutcome>
        where TEvent : Event
    {
        IBuilderWithCondition<TAbility, TEvent, TContext, TOutcome> WithCondition(
            ICondition<TEvent, TContext> condition
        );
    }
}
