namespace Snapdragon.Fluent.Selectors
{
    public record BothSides : ISelector<Player, object>
    {
        public IEnumerable<Player> Get(object context, Game game)
        {
            yield return game.Top;
            yield return game.Bottom;
        }
    }
}
