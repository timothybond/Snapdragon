namespace Snapdragon.Fluent.Selectors
{
    public record SameSide : ISingleItemSelector<Player, IObjectWithSide>
    {
        public Player? GetOrDefault(IObjectWithSide context, Game game)
        {
            return context.Side == Side.Top ? game.TopPlayer : game.BottomPlayer;
        }
    }
}
