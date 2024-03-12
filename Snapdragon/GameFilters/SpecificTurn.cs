namespace Snapdragon.GameFilters
{
    public record SpecificTurn(int Turn) : IGameFilter
    {
        public bool Applies(Game game)
        {
            return game.Turn == Turn;
        }
    }
}
