namespace Snapdragon.Fluent.Builders
{
    public record ConditionBuilder<TAbility, TContext, TOutcome>(
        IResultFactory<TAbility, TContext, TOutcome> Factory
    ) : IConditionBuilder<TAbility, TContext, TOutcome>
    {
        IBuilderWithCondition<TAbility, TContext, TOutcome> IConditionBuilder<
            TAbility,
            TContext,
            TOutcome
        >.WithCondition(ICondition<TContext> condition)
        {
            return new BuilderWithCondition<TAbility, TContext, TOutcome>(condition, Factory);
        }

        public PastEventConditionBuilder<TAbility, TContext, TOutcome> PastEvent
        {
            get { return new PastEventConditionBuilder<TAbility, TContext, TOutcome>(Factory); }
        }
    }
}
