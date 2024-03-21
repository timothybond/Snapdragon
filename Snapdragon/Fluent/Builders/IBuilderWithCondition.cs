namespace Snapdragon.Fluent.Builders
{
    public interface IBuilderWithCondition<TAbility, TContext, TOutcome>
        : IBuilder<TAbility, TContext, TOutcome>
    {
        public IConditionBuilder<TAbility, TContext, TOutcome> And { get; }
    }

    public interface IBuilderWithCondition<TAbility, TEvent, TContext, TOutcome>
        : IBuilder<TAbility, TContext, TOutcome>
        where TEvent : Event
    {
        public IConditionBuilder<TAbility, TEvent, TContext, TOutcome> And { get; }
    }
}
