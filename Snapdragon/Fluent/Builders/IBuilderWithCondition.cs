namespace Snapdragon.Fluent.Builders
{
    public interface IBuilderWithCondition<TAbility, TContext, TOutcome> : IBuilder<TAbility, TContext, TOutcome>
    {
        public IConditionBuilder<TAbility, TContext, TOutcome> And { get; }
    }
}
