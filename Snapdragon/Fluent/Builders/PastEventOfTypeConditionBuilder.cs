using Snapdragon.Fluent.Conditions;

namespace Snapdragon.Fluent.Builders
{
    public record PastEventOfTypeConditionBuilder<TEvent, TAbility, TContext, TOutcome>(
        IConditionBuilder<TAbility, TContext, TOutcome> PriorBuilder
    ) : IBuilder<TAbility, TContext, TOutcome>
        where TEvent : Event
    {
        public TAbility Then(TOutcome outcome)
        {
            return PriorBuilder
                .WithCondition(new PastEventCondition<TEvent, TContext>())
                .Then(outcome);
        }

        public IBuilderWithCondition<TAbility, TContext, TOutcome> Where(
            IEventFilter<TEvent, TContext>? filter
        )
        {
            return PriorBuilder.WithCondition(new PastEventCondition<TEvent, TContext>(filter));
        }
    }

    public record PastEventOfTypeConditionBuilder<TPastEvent, TAbility, TEvent, TContext, TOutcome>(
        IConditionBuilder<TAbility, TEvent, TContext, TOutcome> PriorBuilder
    ) : IBuilder<TAbility, TEvent, TContext, TOutcome>
        where TEvent : Event
        where TPastEvent : Event
    {
        public TAbility Build(TOutcome outcome)
        {
            return PriorBuilder
                .WithCondition(new PastEventCondition<TPastEvent, TContext>())
                .Then(outcome);
        }

        public IBuilderWithCondition<TAbility, TEvent, TContext, TOutcome> Where(
            IEventFilter<TPastEvent, TContext>? filter
        )
        {
            return PriorBuilder.WithCondition(new PastEventCondition<TPastEvent, TContext>(filter));
        }
    }
}
