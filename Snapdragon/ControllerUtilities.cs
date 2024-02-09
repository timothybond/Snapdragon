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
            var possibleActionSets = new List<IReadOnlyList<IPlayerAction>>();
            var possibleMoveSets = GetPossibleMoveActionSets(game, side);

            foreach (var moveSet in possibleMoveSets)
            {
                var gameWithMoves = moveSet.Aggregate(game, (g, move) => move.Apply(g));
                foreach (
                    var possibleCardPlays in GetPossiblePlayCardActionSets(gameWithMoves, side)
                )
                {
                    yield return moveSet.Concat(possibleCardPlays).ToList();
                }
            }
        }

        /// <summary>
        /// Gets all sets of possible <see cref="MoveCardAction"/>s
        /// that are valid for the given game state and player.
        /// </summary>
        public static IEnumerable<IReadOnlyList<IPlayerAction>> GetPossibleMoveActionSets(
            Game game,
            Side side
        )
        {
            var priorMoves = new Stack<MoveCardAction>();
            var skippedCards = new Stack<Card>();
            var results = new List<IReadOnlyList<IPlayerAction>>();

            GetPossibleMoveActionSetsHelper(game, side, priorMoves, skippedCards, results);

            return results;
        }

        public static void GetPossibleMoveActionSetsHelper(
            Game game,
            Side side,
            Stack<MoveCardAction> priorMoves,
            Stack<Card> skippedCards,
            List<IReadOnlyList<IPlayerAction>> results
        )
        {
            var moveableCards = game
                .AllCards.Where(c =>
                    c.Side == side
                    && c.Column is Column column
                    && c.Column.Value.Others().Any(col => game.CanMove(c, col))
                    && !game.GetBlockedEffects(column, side).Contains(EffectType.MoveFromLocation)
                    && !skippedCards.Any(sk => sk.Id == c.Id)
                    && !priorMoves.Any(m => m.Card.Id == c.Id)
                )
                .ToList();

            if (moveableCards.Count == 0)
            {
                results.Add(priorMoves.Reverse().ToList());
                return;
            }

            var currentCard = moveableCards[0];

            if (currentCard.Column == null)
            {
                throw new InvalidOperationException(
                    "Somehow we tried to compute possible move actions for a card without a Column value."
                );
            }

            skippedCards.Push(currentCard);

            // Also always include the possiblity of not moving this card
            GetPossibleMoveActionSetsHelper(game, side, priorMoves, skippedCards, results);

            skippedCards.Pop();

            foreach (var column in All.Columns)
            {
                if (column == currentCard.Column)
                {
                    continue;
                }

                if (!game.CanMove(currentCard, column))
                {
                    continue;
                }

                // TODO: Handle other restrictions on slots
                if (game[column][currentCard.Side].Count >= 4)
                {
                    continue;
                }

                // Apparently this is a valid move
                var move = new MoveCardAction(
                    currentCard.Side,
                    currentCard,
                    currentCard.Column.Value,
                    column
                );
                var gameWithThisMove = move.Apply(game);

                priorMoves.Push(move);

                GetPossibleMoveActionSetsHelper(
                    gameWithThisMove,
                    side,
                    priorMoves,
                    skippedCards,
                    results
                );

                priorMoves.Pop();
            }
        }

        /// <summary>
        /// Gets all sets of possible <see cref="PlayCardAction"/>s
        /// that are valid for the given game state and player.
        /// </summary>
        public static IEnumerable<IReadOnlyList<IPlayerAction>> GetPossiblePlayCardActionSets(
            Game game,
            Side side
        )
        {
            // Every entry in this list is a set of cards we can afford to play
            var playableCardSets = GetPlayableCardSets(game[side]);

            // This is the count of open slots by column,
            // and also where we check if PlayCards is blocked
            var availableColumns = GetPlayableCardSlots(game, side);

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
                    // These are only valid if none of the cards is restricted from play
                    // in the column chosen for it.

                    if (
                        !cardSet
                            .Select(
                                (c, i) =>
                                    c.PlayRestriction?.IsBlocked(game, columnChoices[i], c) ?? false
                            )
                            .Any(blocked => blocked)
                    )
                    {
                        yield return cardSet
                            .Select((c, i) => new PlayCardAction(side, c, columnChoices[i]))
                            .ToList();
                    }
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
        ///
        /// Note this checks whether <see cref="EffectType.PlayCard"/> is blocked. Don't use it for Moves.
        /// </summary>
        /// <returns>A list of <see cref="Column"/>, in order from Left to Right,
        /// with one entry of each value per slot available in that <see cref="Column"/>.</returns>
        public static (int Left, int Middle, int Right) GetPlayableCardSlots(Game game, Side side)
        {
            return (
                GetPlayableCardSlots(game, side, Column.Left),
                GetPlayableCardSlots(game, side, Column.Middle),
                GetPlayableCardSlots(game, side, Column.Right)
            );
        }

        public static int GetPlayableCardSlots(Game game, Side side, Column column)
        {
            return game.GetBlockedEffects(column, side).Contains(EffectType.PlayCard)
                ? 0
                : 4 - game[column][side].Count;
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
