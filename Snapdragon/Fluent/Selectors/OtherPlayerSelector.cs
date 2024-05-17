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

            return game[initialPlayer.Side.Other()];
        }

        public bool Selects(Player item, TContext context, Game game)
        {
            // TODO: Determine if this ever causes a problem by virtue of
            // not being able to select ANY player with the inner selector.
            return !Inner.Selects(item, context, game);
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

            return game[initialPlayer.Side.Other()];
        }
    }
}
