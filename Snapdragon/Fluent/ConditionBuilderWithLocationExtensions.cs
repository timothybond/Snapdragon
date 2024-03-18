using Snapdragon.Fluent.Builders;
using Snapdragon.Fluent.Conditions;

namespace Snapdragon.Fluent
{
    public static class ConditionBuilderWithLocationExtensions
    {
        public static IBuilderWithCondition<TResult, TContext> InColumn<TResult, TContext>(
            this IConditionBuilder<TResult, TContext> builder,
            params Column[] columns
        )
            where TContext : IObjectWithColumn
        {
            return builder.WithCondition(new ColumnCondition<TContext>(columns));
        }

        public static IBuilderWithCondition<TResult, TContext> LocationFull<TResult, TContext>(
            this IConditionBuilder<TResult, TContext> builder
        )
            where TContext : IObjectWithColumn, ICard
        {
            return builder.WithCondition(new LocationFullCondition<TContext>());
        }
    }
}
