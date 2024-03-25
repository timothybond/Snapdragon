using Snapdragon.Fluent.Builders;
using Snapdragon.Fluent.Conditions;

namespace Snapdragon.Fluent
{
    public static class ConditionBuilderExtensions
    {
        public static PastEventConditionBuilder<TAbility, TContext, TOutcome> PastEvent<
            TAbility,
            TContext,
            TOutcome
        >(this IConditionBuilder<TAbility, TContext, TOutcome> builder)
        {
            return new PastEventConditionBuilder<TAbility, TContext, TOutcome>(builder);
        }

        public static PastEventConditionBuilder<TAbility, TEvent, TContext, TOutcome> PastEvent<
            TAbility,
            TEvent,
            TContext,
            TOutcome
        >(this IConditionBuilder<TAbility, TEvent, TContext, TOutcome> builder)
            where TEvent : Event
        {
            return new PastEventConditionBuilder<TAbility, TEvent, TContext, TOutcome>(builder);
        }

        public static NoPastEventConditionBuilder<TAbility, TContext, TOutcome> NoPastEvent<
            TAbility,
            TContext,
            TOutcome
        >(this IConditionBuilder<TAbility, TContext, TOutcome> builder)
        {
            return new NoPastEventConditionBuilder<TAbility, TContext, TOutcome>(builder);
        }

        public static NoPastEventConditionBuilder<TAbility, TEvent, TContext, TOutcome> NoPastEvent<
            TAbility,
            TEvent,
            TContext,
            TOutcome
        >(this IConditionBuilder<TAbility, TEvent, TContext, TOutcome> builder)
            where TEvent : Event
        {
            return new NoPastEventConditionBuilder<TAbility, TEvent, TContext, TOutcome>(builder);
        }

        public static IBuilderWithCondition<TAbility, TContext, TOutcome> AfterTurn<
            TAbility,
            TContext,
            TOutcome
        >(this IConditionBuilder<TAbility, TContext, TOutcome> builder, int turn)
            where TContext : class
        {
            return builder.WithCondition(new AfterTurnCondition(turn));
        }

        public static IBuilderWithCondition<TAbility, TEvent, TContext, TOutcome> AfterTurn<
            TAbility,
            TEvent,
            TContext,
            TOutcome
        >(this IConditionBuilder<TAbility, TEvent, TContext, TOutcome> builder, int turn)
            where TContext : class where TEvent : Event
        {
            return builder.WithCondition(new AfterTurnCondition(turn));
        }
    }
}
