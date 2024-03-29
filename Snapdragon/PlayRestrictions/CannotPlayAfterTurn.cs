namespace Snapdragon.PlayRestrictions
{
    public record CannotPlayAfterTurn(int Turn) : IPlayRestriction
    {
        public bool IsBlocked(Game game, Column column, ICardInstance source)
        {
            return game.Turn > Turn;
        }
    }
}
