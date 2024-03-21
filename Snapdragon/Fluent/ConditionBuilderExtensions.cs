using Snapdragon.Fluent.Builders;

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
    }
}
