using Snapdragon.Fluent.Builders;
using Snapdragon.Fluent.Conditions;

namespace Snapdragon.Fluent
{
    public static class ConditionBuilderWithLocationExtensions
    {
        public static IBuilderWithCondition<TAbility, TContext, TOutcome> InColumn<
            TAbility,
            TContext,
            TOutcome
        >(this IConditionBuilder<TAbility, TContext, TOutcome> builder, params Column[] columns)
            where TContext : IObjectWithColumn
        {
            return builder.WithCondition(new ColumnCondition<TContext>(columns));
        }

        public static IBuilderWithCondition<TAbility, TContext, TOutcome> LocationFull<
            TAbility,
            TContext,
            TOutcome
        >(this IConditionBuilder<TAbility, TContext, TOutcome> builder)
            where TContext : IObjectWithColumn, ICardInstance
        {
            return builder.WithCondition(new LocationFullCondition<TContext>());
        }
    }
}
