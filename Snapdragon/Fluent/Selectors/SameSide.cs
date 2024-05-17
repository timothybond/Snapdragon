namespace Snapdragon.Fluent.Selectors
{
    public record SameSide : ISingleItemSelector<Player, IObjectWithSide>
    {
        public Player? GetOrDefault(IObjectWithSide context, Game game)
        {
            return game[context.Side];
        }

        public bool Selects(Player item, IObjectWithSide context, Game game)
        {
            return item.Side == context.Side;
        }
    }
}
