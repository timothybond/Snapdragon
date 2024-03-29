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
        /// Note this is equivalent to <see cref="Here{TContext}(ISelector{ICardInstance, TContext})"/>,
        /// but that function can only be invoked on selectors that already have an appropriate
        /// context, which isn't always the case (such as anything that retrieves all cards
        /// regardless of the context and therefore has <see cref="object"/> as its context type).
        /// </summary>
        public static ISelector<ICardInstance, Location> AtLocation(this ISelector<ICardInstance, object> selector)
        {
            return new FilteredSelector<ICardInstance, Location>(selector, new HereFilter());
        }

        public static ISelector<ICardInstance, TContext> PlayedThisTurn<TContext>(
            this ISelector<ICardInstance, TContext> selector
        )
        {
            return new FilteredSelector<ICardInstance, TContext>(selector, new PlayedThisTurn<TContext>());
        }

        public static ISelector<ICardInstance, TContext> Here<TContext>(
            this ISelector<ICardInstance, TContext> selector
        )
            where TContext : class, IObjectWithPossibleColumn
        {
            return new FilteredSelector<ICardInstance, TContext>(selector, new HereFilter());
        }

        public static ISelector<ICardInstance, TContext> WithMaxPower<TContext>(
            this ISelector<ICardInstance, TContext> selector
        )
            where TContext : class
        {
            return new FilteredSelector<ICardInstance, TContext>(selector, new MaxPowerOf());
        }

        public static ISelector<ICardInstance, TContext> WithMinPower<TContext>(
            this ISelector<ICardInstance, TContext> selector
        )
            where TContext : class
        {
            return new FilteredSelector<ICardInstance, TContext>(selector, new MinPowerOf());
        }

        public static ISelector<ICardInstance, TContext> WithMaxCost<TContext>(
            this ISelector<ICardInstance, TContext> selector
        )
            where TContext : class
        {
            return new FilteredSelector<ICardInstance, TContext>(selector, new MaxCostOf());
        }

        public static ISelector<ICardInstance, TContext> WithMinCost<TContext>(
            this ISelector<ICardInstance, TContext> selector
        )
            where TContext : class
        {
            return new FilteredSelector<ICardInstance, TContext>(selector, new MinCostOf());
        }

        public static ISelector<ICardInstance, TContext> WithCost<TContext>(
            this ISelector<ICardInstance, TContext> selector,
            int cost
        )
            where TContext : class
        {
            return new FilteredSelector<ICardInstance, TContext>(selector, new WithCost(cost));
        }

        public static ISelector<ICardInstance, TContext> WithOngoingAbilities<TContext>(
            this ISelector<ICardInstance, TContext> selector
        )
            where TContext : class
        {
            return new FilteredSelector<ICardInstance, TContext>(selector, new WithOngoingAbilities());
        }

        public static CardPower<TContext> Power<TContext>(
            this ISingleItemSelector<ICardInstance, TContext> selector
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

        #region Player Selectors

        public static ISelector<Player, TEvent, TContext> Other<TEvent, TContext>(
            this ISingleItemSelector<Player, TEvent, TContext> initialSelector
        )
            where TEvent : Event
        {
            return new OtherPlayerSelector<TEvent, TContext>(initialSelector);
        }

        public static ISelector<Player, TContext> Other<TContext>(
            this ISingleItemSelector<Player, TContext> initialSelector
        )
        {
            return new OtherPlayerSelector<TContext>(initialSelector);
        }

        #endregion

        #region General Selectors

        public static ISelector<TSelected, TContext> ForPlayer<TSelected, TContext>(
            this ISelector<TSelected, TContext> initial,
            ISingleItemSelector<Player, TContext> playerSelector
        )
            where TSelected : IObjectWithSide
        {
            return new FilteredSelector<TSelected, TContext>(
                initial,
                new PlayerFilter<TSelected, TContext>(playerSelector)
            );
        }

        public static ISingleItemSelector<ICardInstance, TContext> First<TContext>(
            this ISelector<ICardInstance, TContext> selector
        )
            where TContext : class
        {
            return new FilteredSingleItemSelector<ICardInstance, TContext>(selector, new FirstOf<ICardInstance>());
        }

        public static ISingleItemSelector<ICardInstance, TContext> Last<TContext>(
            this ISelector<ICardInstance, TContext> selector
        )
            where TContext : class
        {
            return new FilteredSingleItemSelector<ICardInstance, TContext>(selector, new LastOf<ICardInstance>());
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
