using Snapdragon.Fluent.Filters;
using Snapdragon.Fluent.Selectors;

namespace Snapdragon.Fluent
{
    public static class FilterExtensions
    {
        public static ISingleItemFilter<TSelected, TContext> GetRandom<TSelected, TContext>(
            this IFilter<TSelected, TContext> selector
        )
        {
            return new ChainedSingleItemFilter<TSelected, TContext>(
                selector,
                new RandomSingleItem<TSelected, TContext>()
            );
        }
    }
}
