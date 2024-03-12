namespace Snapdragon.GameFilters
{
    public record AfterTurn(int Turn) : IGameFilter
    {
        public bool Applies(Game game)
        {
            return game.Turn > Turn;
        }
    }
}
