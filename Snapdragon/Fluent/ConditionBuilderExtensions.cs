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
    }
}
