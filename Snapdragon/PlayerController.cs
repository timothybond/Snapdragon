namespace Snapdragon
{
    public interface IPlayerController
    {
        IReadOnlyList<IPlayerAction> GetActions(
            GameState gameState,
            Side player,
            Side firstPlayerToResolve
        );
    }
}
