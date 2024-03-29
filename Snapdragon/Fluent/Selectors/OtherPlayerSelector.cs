namespace Snapdragon.Fluent.Selectors
{
    public record OtherPlayerSelector<TContext>(ISingleItemSelector<Player, TContext> Inner)
        : ISingleItemSelector<Player, TContext>
    {
        public Player? GetOrDefault(TContext context, Game game)
        {
            var initialPlayer = Inner.GetOrDefault(context, game);

            if (initialPlayer == null)
            {
                return null;
            }

            return initialPlayer.Side == Side.Top ? game.BottomPlayer : game.TopPlayer;
        }
    }

    public record OtherPlayerSelector<TEvent, TContext>(
        ISingleItemSelector<Player, TEvent, TContext> Inner
    ) : ISingleItemSelector<Player, TEvent, TContext>
        where TEvent : Event
    {
        public Player? GetOrDefault(TEvent e, TContext context, Game game)
        {
            var initialPlayer = Inner.GetOrDefault(e, context, game);

            if (initialPlayer == null)
            {
                return null;
            }

            return initialPlayer.Side == Side.Top ? game.BottomPlayer : game.TopPlayer;
        }
    }
}
