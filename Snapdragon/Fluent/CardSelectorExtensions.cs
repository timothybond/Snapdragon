using Snapdragon.Fluent.Calculations;
using Snapdragon.Fluent.Filters;
using Snapdragon.Fluent.Selectors;

namespace Snapdragon.Fluent
{
    public static class CardSelectorExtensions
    {
        public static CardPower<TContext> Power<TContext>(
            this ISingleCardSelector<TContext> selector
        )
            where TContext : class
        {
            return new CardPower<TContext>(selector);
        }

        public static ICardSelector<TContext> WithCost<TContext>(
            this ICardSelector<TContext> selector,
            int cost
        )
            where TContext : class
        {
            return new FilteredCardSelector<TContext>(selector, new WithCost(cost));
        }

        public static ICalculation<TContext> Count<TContext>(this ICardSelector<TContext> selector)
            where TContext : class
        {
            return new CardCount<TContext>(selector);
        }
    }
}
