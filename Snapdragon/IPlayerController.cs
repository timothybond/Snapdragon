namespace Snapdragon
{
    public interface IPlayerController
    {
        Task<IReadOnlyList<IPlayerAction>> GetActions(Game game, Side side);
    }
}
