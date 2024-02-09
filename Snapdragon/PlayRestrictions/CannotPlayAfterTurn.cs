namespace Snapdragon.PlayRestrictions
{
    public record CannotPlayAfterTurn(int Turn) : IPlayRestriction
    {
        public bool IsBlocked(Game game, Column column, Card source)
        {
            return game.Turn > Turn;
        }
    }
}
