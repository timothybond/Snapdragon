using Snapdragon.Fluent.Calculations;

namespace Snapdragon.Fluent
{
    public static class CardSelectorExtensions
    {
        public static CardPower<TContext> Power<TContext>(this ICardSelector<TContext> selector)
            where TContext : class
        {
            return new CardPower<TContext>(selector);
        }
    }
}
