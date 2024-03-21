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
    }

    public record ConditionBuilder<TAbility, TEvent, TContext, TOutcome>(
        IResultFactory<TAbility, TEvent, TContext, TOutcome> Factory
    ) : IConditionBuilder<TAbility, TEvent, TContext, TOutcome>
        where TEvent : Event
    {
        IBuilderWithCondition<TAbility, TEvent, TContext, TOutcome> IConditionBuilder<
            TAbility,
            TEvent,
            TContext,
            TOutcome
        >.WithCondition(ICondition<TEvent, TContext> condition)
        {
            return new BuilderWithCondition<TAbility, TEvent, TContext, TOutcome>(
                condition,
                Factory
            );
        }
    }
}
