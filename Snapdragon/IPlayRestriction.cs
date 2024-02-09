namespace Snapdragon
{
    public interface IPlayRestriction
    {
        bool IsBlocked(Game game, Column column, Card source);
    }
}
