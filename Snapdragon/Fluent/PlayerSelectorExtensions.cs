using Snapdragon.Fluent.Selectors;

namespace Snapdragon.Fluent
{
    public static class PlayerSelectorExtensions
    {
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
    }
}
