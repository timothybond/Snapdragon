using Snapdragon.Fluent.Calculations;
using Snapdragon.Fluent.Filters;
using Snapdragon.Fluent.Selectors;

namespace Snapdragon.Fluent
{
    public static class SelectorExtensions
    {
        #region Card Selectors

        /// <summary>
        /// Gets only those cards that are at the context <see cref="Location"/>.
        ///
        /// Note this is equivalent to <see cref="Here{TContext}(ISelector{ICard, TContext})"/>,
        /// but that function can only be invoked on selectors that already have an appropriate
        /// context, which isn't always the case (such as anything that retrieves all cards
        /// regardless of the context and therefore has <see cref="object"/> as its context type).
        /// </summary>
        public static ISelector<ICard, Location> AtLocation(this ISelector<ICard, object> selector)
        {
            return new FilteredSelector<ICard, Location>(selector, new HereFilter());
        }

        public static ISelector<ICard, TContext> Here<TContext>(
            this ISelector<ICard, TContext> selector
        )
            where TContext : class, IObjectWithPossibleColumn
        {
            return new FilteredSelector<ICard, TContext>(selector, new HereFilter());
        }

        public static ISelector<ICard, TContext> WithMaxPower<TContext>(
            this ISelector<ICard, TContext> selector
        )
            where TContext : class
        {
            return new FilteredSelector<ICard, TContext>(selector, new MaxPowerOf());
        }

        public static ISelector<ICard, TContext> WithMinPower<TContext>(
            this ISelector<ICard, TContext> selector
        )
            where TContext : class
        {
            return new FilteredSelector<ICard, TContext>(selector, new MinPowerOf());
        }

        public static ISelector<ICard, TContext> WithMaxCost<TContext>(
            this ISelector<ICard, TContext> selector
        )
            where TContext : class
        {
            return new FilteredSelector<ICard, TContext>(selector, new MaxCostOf());
        }

        public static ISelector<ICard, TContext> WithMinCost<TContext>(
            this ISelector<ICard, TContext> selector
        )
            where TContext : class
        {
            return new FilteredSelector<ICard, TContext>(selector, new MinCostOf());
        }

        public static ISelector<ICard, TContext> WithCost<TContext>(
            this ISelector<ICard, TContext> selector,
            int cost
        )
            where TContext : class
        {
            return new FilteredSelector<ICard, TContext>(selector, new WithCost(cost));
        }

        public static ISelector<ICard, TContext> WithOngoingAbilities<TContext>(
            this ISelector<ICard, TContext> selector
        )
            where TContext : class
        {
            return new FilteredSelector<ICard, TContext>(selector, new WithOngoingAbilities());
        }

        public static CardPower<TContext> Power<TContext>(
            this ISingleItemSelector<ICard, TContext> selector
        )
            where TContext : class
        {
            return new CardPower<TContext>(selector);
        }

        #endregion

        #region Location Selectors

        public static ISelector<Location, TContext> WithOpenSlots<TContext>(
            this ISelector<Location, TContext> locationSelector,
            ISingleItemSelector<Player, TContext> playerSelector
        )
        {
            return new FilteredSelector<Location, TContext>(
                locationSelector,
                new LocationsWithOpenSlots<TContext>(playerSelector)
            );
        }

        #endregion

        #region Card Definition Selectors

        public static ISelector<CardDefinition, TContext> WithCost<TContext>(
            this ISelector<CardDefinition, TContext> selector,
            int cost
        )
            where TContext : class
        {
            return new FilteredSelector<CardDefinition, TContext>(selector, new WithCost(cost));
        }

        #endregion

        #region General Selectors

        public static ISingleItemSelector<ICard, TContext> First<TContext>(
            this ISelector<ICard, TContext> selector
        )
            where TContext : class
        {
            return new FilteredSingleItemSelector<ICard, TContext>(selector, new FirstOf<ICard>());
        }

        public static ISingleItemSelector<ICard, TContext> Last<TContext>(
            this ISelector<ICard, TContext> selector
        )
            where TContext : class
        {
            return new FilteredSingleItemSelector<ICard, TContext>(selector, new LastOf<ICard>());
        }

        public static ISingleItemSelector<TSelected, TContext> GetRandom<TSelected, TContext>(
            this ISelector<TSelected, TContext> selector
        )
        {
            return new FilteredSingleItemSelector<TSelected, TContext>(
                selector,
                new RandomSingleItem<TSelected, TContext>()
            );
        }

        public static ISelector<TSelected, TContext> GetRandom<TSelected, TContext>(
            this ISelector<TSelected, TContext> selector,
            int number
        )
        {
            return new FilteredSelector<TSelected, TContext>(
                selector,
                new RandomItems<TSelected, TContext>(number)
            );
        }

        public static ICalculation<TContext> Count<TSelected, TContext>(
            this ISelector<TSelected, TContext> selector
        )
            where TContext : class
        {
            return new ItemCount<TSelected, TContext>(selector);
        }

        #endregion
    }
}
