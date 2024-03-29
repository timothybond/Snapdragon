namespace Snapdragon.PlayRestrictions
{
    public record CannotPlayInColumn(Column Column) : IPlayRestriction
    {
        public bool IsBlocked(Game game, Column column, ICardInstance source)
        {
            return Column == column;
        }
    }
}
