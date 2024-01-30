using Snapdragon.PlayerActions;

namespace Snapdragon
{
    /// <summary>
    /// An implementation of <see cref="IPlayerController"/> that essentially just plays cards in random locations.
    /// </summary>
    public class RandomPlayerController : IPlayerController
    {
        public IReadOnlyList<IPlayerAction> GetActions(GameState gameState, Side player)
        {
            var cardsToPlay = GetRandomPlayableCards(gameState[player]);

            var availableColumns = new List<Column>();

            // Still need to avoid trying to play over full lanes
            foreach (var column in new[] { Column.Left, Column.Middle, Column.Right })
            {
                for (var i = 0; i < 4 - gameState[column][player].Count; i++)
                {
                    availableColumns.Add(column);
                }
            }

            availableColumns = availableColumns.OrderBy(c => Random.Next()).ToList();
            cardsToPlay = cardsToPlay.Take(availableColumns.Count).ToList();

            return cardsToPlay
                .Select((c, i) => new PlayCardAction(player, c, availableColumns[i]))
                .ToList();
        }

        private IReadOnlyList<Card> GetRandomPlayableCards(Player player)
        {
            var cardsToPlay = new List<Card>();
            var energy = player.Energy;
            var hand = player.Hand;

            var playableCards = hand.Where(c => c.Cost <= energy).OrderBy(c => Random.Next()).ToList();

            while (playableCards.Count > 0)
            {
                var card = playableCards.First();
                energy = energy - card.Cost;

                cardsToPlay.Add(card);

                playableCards = playableCards
                    .Skip(1)
                    .Where(c => c.Cost <= energy)
                    .OrderBy(c => Random.Next())
                    .ToList();
            }

            return cardsToPlay;
        }
    }
}
