using static Snapdragon.ControllerUtilities;

namespace Snapdragon
{
    /// <summary>
    /// An implementation of <see cref="IPlayerController"/> that essentially just makes random moves.
    /// </summary>
    public class RandomPlayerController : IPlayerController
    {
        public IReadOnlyList<IPlayerAction> GetActions(Game game, Side side)
        {
            return Random.Of(GetPossibleActionSets(game, side).ToList());
        }
    }
}
