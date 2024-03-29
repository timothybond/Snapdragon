namespace Snapdragon
{
    public interface IPlayRestriction
    {
        bool IsBlocked(Game game, Column column, ICardInstance source);
    }
}
