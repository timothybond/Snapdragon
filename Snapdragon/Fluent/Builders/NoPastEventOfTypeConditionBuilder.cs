﻿using Snapdragon.Fluent.Conditions;

namespace Snapdragon.Fluent.Builders
{
    public record NoPastEventOfTypeConditionBuilder<TEvent, TAbility, TContext, TOutcome>(
        IConditionBuilder<TAbility, TContext, TOutcome> PriorBuilder
    ) : IBuilder<TAbility, TContext, TOutcome>
        where TEvent : Event
    {
        public TAbility Build(TOutcome outcome)
        {
            return PriorBuilder
                .WithCondition(new PastEventCondition<TEvent, TContext>())
                .Build(outcome);
        }

        public IBuilderWithCondition<TAbility, TContext, TOutcome> Where(
            IEventFilter<TEvent, TContext>? filter
        )
        {
            return PriorBuilder.WithCondition(new PastEventCondition<TEvent, TContext>(filter));
        }
    }
}
