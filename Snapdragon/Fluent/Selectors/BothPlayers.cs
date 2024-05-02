namespace Snapdragon.Fluent.Selectors
{
    public record BothPlayers : ISelector<Player, object>
    {
        public IEnumerable<Player> Get(object context, Game game)
        {
            yield return game.TopPlayer;
            yield return game.BottomPlayer;
        }

        public bool Selects(Player item, object context, Game game)
        {
            return true;
        }
    }
}
