using Snapdragon.Fluent.Filters;
using Snapdragon.Fluent.Selectors;

namespace Snapdragon.Fluent
{
    public static class SelectorExtensions
    {
        public static ICardSelector<TContext> WithMaxCost<TContext>(
            this ICardSelector<TContext> selector
        )
            where TContext : class
        {
            return new FilteredCardSelector<TContext>(selector, new MaxCostOf());
        }

        public static ICardSelector<TContext> WithMinCost<TContext>(
            this ICardSelector<TContext> selector
        )
            where TContext : class
        {
            return new FilteredCardSelector<TContext>(selector, new MinCostOf());
        }

        public static ICardSelector<TContext> Last<TContext>(this ICardSelector<TContext> selector)
            where TContext : class
        {
            return new FilteredCardSelector<TContext>(selector, new LastOf());
        }

        public static RandomCard<TContext> GetRandom<TContext>(
            this ICardSelector<TContext> selector,
            int number = 1
        )
        {
            return new RandomCard<TContext>(selector, number);
        }
    }
}
