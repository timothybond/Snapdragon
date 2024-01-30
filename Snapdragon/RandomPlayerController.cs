using Snapdragon.PlayerActions;
using static Snapdragon.ControllerUtilities;

namespace Snapdragon
{
    /// <summary>
    /// An implementation of <see cref="IPlayerController"/> that essentially just plays cards in random locations.
    /// </summary>
    public class RandomPlayerController : IPlayerController
    {
        public IReadOnlyList<IPlayerAction> GetActions(Game game, Side side)
        {
            // TODO: Also have Move actions
            var playableCardSets = GetPlayableCardSets(game[side]);
            var availableSlots = GetAvailableCardSlots(game, side);
            var totalSlots = availableSlots.Left + availableSlots.Middle + availableSlots.Right;

            // Ignore card sets with too many cards (note than an empty set is always present)
            playableCardSets = playableCardSets.Where(s => s.Count <= totalSlots).ToList();

            var cardsToPlay = Random.Of(playableCardSets);

            var columns = GetRandomColumns(cardsToPlay.Count, availableSlots);

            return cardsToPlay.Select((c, i) => new PlayCardAction(side, c, columns[i])).ToList();
        }
    }
}
