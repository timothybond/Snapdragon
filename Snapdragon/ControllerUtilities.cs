using Snapdragon.PlayerActions;

namespace Snapdragon
{
    public static class ControllerUtilities
    {
        public static IEnumerable<IReadOnlyList<IPlayerAction>> GetPossibleActionSets(
            Game game,
            Side side
        )
        {
            // TODO: Also have Move actions
            var playableCardSets = GetPlayableCardSets(game[side]);
            var availableColumns = GetAvailableCardSlots(game, side);
            var totalAvailableSlots =
                availableColumns.Left + availableColumns.Middle + availableColumns.Right;

            // Ignore card sets with too many cards (note than an empty set is always present)
            playableCardSets = playableCardSets.Where(s => s.Count <= totalAvailableSlots).ToList();

            var columnChoicesByCount = new Dictionary<int, IReadOnlyList<IReadOnlyList<Column>>>();

            foreach (var cardSet in playableCardSets)
            {
                if (!columnChoicesByCount.ContainsKey(cardSet.Count))
                {
                    columnChoicesByCount[cardSet.Count] = GetPossibleColumnChoices(
                        cardSet.Count,
                        availableColumns
                    );
                }

                foreach (var columnChoices in columnChoicesByCount[cardSet.Count])
                {
                    yield return cardSet
                        .Select((c, i) => new PlayCardAction(side, c, columnChoices[i]))
                        .ToList();
                }
            }
        }

        /// <summary>
        /// Gets all possible permutations of <see cref="Column"/> choices for the next set of actions.
        /// </summary>
        /// <param name="total">Total number of slots to be filled.</param>
        /// <param name="available">Available slots to fill.</param>
        /// <returns></returns>
        public static IReadOnlyList<IReadOnlyList<Column>> GetPossibleColumnChoices(
            int total,
            (int Left, int Middle, int Right) available
        )
        {
            var results = new List<IReadOnlyList<Column>>();
            var stack = new Stack<Column>();

            GetPossibleColumnChoicesHelper(
                total,
                available.Left,
                available.Middle,
                available.Right,
                results,
                stack
            );

            return results;
        }

        /// <summary>
        /// Populates the results list for <see cref="GetPossibleColumnChoices(int, ValueTuple{int, int, int})"/>.
        /// </summary>
        private static void GetPossibleColumnChoicesHelper(
            int remaining,
            int left,
            int middle,
            int right,
            List<IReadOnlyList<Column>> results,
            Stack<Column> stack
        )
        {
            if (remaining == 0)
            {
                results.Add(stack.ToList());
            }

            if (left > 0)
            {
                stack.Push(Column.Left);
                GetPossibleColumnChoicesHelper(
                    remaining - 1,
                    left - 1,
                    middle,
                    right,
                    results,
                    stack
                );
                stack.Pop();
            }

            if (middle > 0)
            {
                stack.Push(Column.Middle);
                GetPossibleColumnChoicesHelper(
                    remaining - 1,
                    left,
                    middle - 1,
                    right,
                    results,
                    stack
                );
                stack.Pop();
            }

            if (right > 0)
            {
                stack.Push(Column.Right);
                GetPossibleColumnChoicesHelper(
                    remaining - 1,
                    left,
                    middle,
                    right - 1,
                    results,
                    stack
                );
                stack.Pop();
            }
        }

        public static IReadOnlyList<Column> GetRandomColumns(
            int total,
            (int Left, int Middle, int Right) available
        )
        {
            // TODO: Make this more performant, if it matters later on
            var columns = new List<Column>();

            columns.AddRange(Enumerable.Repeat(Column.Left, available.Left));
            columns.AddRange(Enumerable.Repeat(Column.Middle, available.Middle));
            columns.AddRange(Enumerable.Repeat(Column.Right, available.Right));

            return columns.OrderBy(c => Random.Next()).Take(total).ToList();
        }

        /// <summary>
        /// Gets a list of all of the slots that cards can still be played in.
        /// </summary>
        /// <returns>A list of <see cref="Column"/>, in order from Left to Right,
        /// with one entry of each value per slot available in that <see cref="Column"/>.</returns>
        public static (int Left, int Middle, int Right) GetAvailableCardSlots(Game game, Side side)
        {
            var availableColumns = new List<Column>();
            var left = 4 - game[Column.Left][side].Count;
            var middle = 4 - game[Column.Middle][side].Count;
            var right = 4 - game[Column.Right][side].Count;

            return (left, middle, right);
        }

        /// <summary>
        /// Gets all distinct combinations of cards that can be played out of the given <see cref="Player"/>'s hand.
        ///
        /// Order does not matter.
        ///
        /// Public for testing purposes.
        /// </summary>
        public static IReadOnlyList<IReadOnlyList<Card>> GetPlayableCardSets(Player player)
        {
            return GetPlayableCardSets(player.Energy, player.Hand);
        }

        private static IReadOnlyList<IReadOnlyList<Card>> GetPlayableCardSets(
            int energy,
            IReadOnlyList<Card> hand
        )
        {
            var results = new List<IReadOnlyList<Card>>();

            // Can always just play no cards.
            results.Add(new List<Card>());

            var playableCards = hand.Where(c => c.Cost <= energy).ToList();

            // Repeatedly build sets of cards that can be played in addition to the first card,
            // and then remove that first card, until there are no cards left.
            while (playableCards.Count > 0)
            {
                var first = playableCards.First();
                playableCards = playableCards.Skip(1).ToList();

                results.AddRange(
                    GetPlayableCardSets(energy - first.Cost, playableCards)
                        .Select(cards => cards.Concat(new[] { first }).ToList())
                );
            }

            return results;
        }
    }
}
