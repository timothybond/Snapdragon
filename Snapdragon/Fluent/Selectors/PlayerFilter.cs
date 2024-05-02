namespace Snapdragon.Fluent.Selectors
{
    public record PlayerFilter<TSelected, TContext>(
        ISingleItemSelector<Player, TContext> PlayerSelector
    ) : IFilter<TSelected, TContext>
        where TSelected : IObjectWithSide
    {
        public bool Applies(TSelected item, TContext context, Game game)
        {
            var player = game[item.Side].Player;
            return PlayerSelector.Selects(player, context, game);
        }

        public IEnumerable<TSelected> GetFrom(
            IEnumerable<TSelected> initial,
            TContext context,
            Game game
        )
        {
            var player = PlayerSelector.GetOrDefault(context, game);

            if (player == null)
            {
                return new List<TSelected>();
            }
            else
            {
                return initial.Where(item => item.Side == player.Side);
            }
        }
    }
}
