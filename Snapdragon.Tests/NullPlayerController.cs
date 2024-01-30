namespace Snapdragon.Tests
{
    /// <summary>
    /// A test <see cref="IPlayerController"/> that just doesn't do anything on its turn.
    /// </summary>
    public class NullPlayerController : IPlayerController
    {
        public IReadOnlyList<IPlayerAction> GetActions(Game game, Side player)
        {
            return new List<IPlayerAction>();
        }
    }
}
