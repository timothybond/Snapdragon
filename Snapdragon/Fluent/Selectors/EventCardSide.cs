namespace Snapdragon.Fluent.Selectors
{
    public record EventCardSide<TContext> : ISingleItemSelector<Player, CardEvent, TContext>
    {
        public Player GetOrDefault(CardEvent e, TContext context, Game game)
        {
            return e.Card.Side == Side.Top ? game.TopPlayer : game.BottomPlayer;
        }
    }
}
