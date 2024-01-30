namespace Snapdragon
{
    public interface IPlayerController
    {
        IReadOnlyList<IPlayerAction> GetActions(Game game, Side player);
    }
}
