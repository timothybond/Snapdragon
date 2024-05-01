using Snapdragon.Fluent;
using Snapdragon.Fluent.Ongoing;
using Snapdragon.GameKernelAccessors;
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
            var cardsWithMoveAbilities = new List<ICard>();
            var cardsWithLocationEffectBlocks = new List<ICard>();
            var cardsWithCardEffectBlocks = new List<ICard>();
            var locationsWithLocationEffectBlocks = new List<Location>();

            foreach (var card in game.AllCards)
            {
                if (card.MoveAbility != null)
                {
                    cardsWithMoveAbilities.Add(card);
                }

                if (card.Ongoing != null)
                {
                    if (card.Ongoing is OngoingBlockLocationEffect<ICard>)
                    {
                        cardsWithLocationEffectBlocks.Add(card);
                    }
                    else if (card.Ongoing is OngoingBlockCardEffect<ICard>)
                    {
                        cardsWithCardEffectBlocks.Add(card);
                    }
                }
            }

            foreach (var location in game.Locations)
            {
                if (
                    location.Definition.Ongoing != null
                    && location.Definition.Ongoing is OngoingBlockLocationEffect<Location>
                )
                {
                    locationsWithLocationEffectBlocks.Add(location);
                }
            }

            var possibleActionSets = new List<IReadOnlyList<IPlayerAction>>();
            var possibleMoveSets = GetPossibleMoveActionSets(
                game,
                side,
                cardsWithMoveAbilities,
                cardsWithLocationEffectBlocks,
                cardsWithCardEffectBlocks,
                locationsWithLocationEffectBlocks
            );

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
        public static IReadOnlyList<IReadOnlyList<IPlayerAction>> GetPossibleMoveActionSets(
            Game game,
            Side side,
            IReadOnlyList<ICard> cardsWithMoveAbilities,
            IReadOnlyList<ICard> cardsWithLocationEffectBlocks,
            IReadOnlyList<ICard> cardsWithCardEffectBlocks,
            IReadOnlyList<Location> locationsWithLocationEffectBlocks
        )
        {
            var priorMoves = new Stack<MoveCardAction>();
            var skippedCards = new Stack<ICard>();
            var results = new List<IReadOnlyList<IPlayerAction>>();

            var blockedEffectsByColumn = game.GetBlockedEffectsByColumn(
                cardsWithLocationEffectBlocks,
                locationsWithLocationEffectBlocks,
                side
            );

            // We assume no cards will BECOME moveable, by this definition, as a result of another card moving.
            // At the moment I don't know of any scenario where that could happen.
            // This is mostly a performance optimization.
            var moveableCards = new List<ICard>();

            var sensorsWithMoveAbilities = game
                .AllSensors.Where(s => s.MoveAbility != null)
                .ToList();

            foreach (var card in game.Left[side])
            {
                if (
                    game.CanMove(
                        card,
                        Column.Middle,
                        blockedEffectsByColumn,
                        cardsWithMoveAbilities,
                        sensorsWithMoveAbilities,
                        cardsWithCardEffectBlocks
                    )
                    || game.CanMove(
                        card,
                        Column.Right,
                        blockedEffectsByColumn,
                        cardsWithMoveAbilities,
                        sensorsWithMoveAbilities,
                        cardsWithCardEffectBlocks
                    )
                )
                {
                    moveableCards.Add(card);
                }
            }

            foreach (var card in game.Middle[side])
            {
                if (
                    game.CanMove(
                        card,
                        Column.Left,
                        blockedEffectsByColumn,
                        cardsWithMoveAbilities,
                        sensorsWithMoveAbilities,
                        cardsWithCardEffectBlocks
                    )
                    || game.CanMove(
                        card,
                        Column.Right,
                        blockedEffectsByColumn,
                        cardsWithMoveAbilities,
                        sensorsWithMoveAbilities,
                        cardsWithCardEffectBlocks
                    )
                )
                {
                    moveableCards.Add(card);
                }
            }

            foreach (var card in game.Right[side])
            {
                if (
                    game.CanMove(
                        card,
                        Column.Left,
                        blockedEffectsByColumn,
                        cardsWithMoveAbilities,
                        sensorsWithMoveAbilities,
                        cardsWithCardEffectBlocks
                    )
                    || game.CanMove(
                        card,
                        Column.Middle,
                        blockedEffectsByColumn,
                        cardsWithMoveAbilities,
                        sensorsWithMoveAbilities,
                        cardsWithCardEffectBlocks
                    )
                )
                {
                    moveableCards.Add(card);
                }
            }

            GetPossibleMoveActionSetsHelper(
                game,
                side,
                moveableCards,
                cardsWithMoveAbilities,
                sensorsWithMoveAbilities,
                cardsWithLocationEffectBlocks,
                blockedEffectsByColumn,
                cardsWithCardEffectBlocks,
                priorMoves,
                skippedCards,
                results
            );

            return results;
        }

        public static void GetPossibleMoveActionSetsHelper(
            Game game,
            Side side,
            IReadOnlyList<ICard> moveableCards,
            IReadOnlyList<ICard> cardsWithMoveAbilities,
            IReadOnlyList<Sensor<ICard>> sensorsWithMoveAbilities,
            IReadOnlyList<ICard> cardsWithLocationEffectBlocks,
            IReadOnlyDictionary<Column, IReadOnlySet<EffectType>> blockedEffectsByColumn,
            IReadOnlyList<ICard> cardsWithCardEffectBlocks,
            Stack<MoveCardAction> priorMoves,
            Stack<ICard> skippedCards,
            List<IReadOnlyList<IPlayerAction>> results
        )
        {
            var moveableCard = moveableCards.FirstOrDefault(c =>
                !skippedCards.Any(sk => sk.Id == c.Id) && !priorMoves.Any(m => m.Card.Id == c.Id)
            );

            if (moveableCard == null)
            {
                results.Add(priorMoves.Reverse().ToList());
                return;
            }

            skippedCards.Push(moveableCard);

            // Also always include the possiblity of not moving this card
            GetPossibleMoveActionSetsHelper(
                game,
                side,
                moveableCards,
                cardsWithMoveAbilities,
                sensorsWithMoveAbilities,
                cardsWithLocationEffectBlocks,
                blockedEffectsByColumn,
                cardsWithCardEffectBlocks,
                priorMoves,
                skippedCards,
                results
            );

            skippedCards.Pop();

            foreach (var column in All.Columns)
            {
                if (column == moveableCard.Column)
                {
                    continue;
                }

                if (
                    !game.CanMove(
                        moveableCard,
                        column,
                        blockedEffectsByColumn,
                        cardsWithMoveAbilities,
                        sensorsWithMoveAbilities,
                        cardsWithCardEffectBlocks
                    )
                )
                {
                    continue;
                }

                // TODO: Handle other restrictions on slots
                if (game[column][moveableCard.Side].Count >= 4)
                {
                    continue;
                }

                // Apparently this is a valid move
                var move = new MoveCardAction(
                    moveableCard.Side,
                    moveableCard,
                    moveableCard.Column,
                    column
                );
                var gameWithThisMove = move.Apply(game);

                priorMoves.Push(move);

                GetPossibleMoveActionSetsHelper(
                    gameWithThisMove,
                    side,
                    moveableCards,
                    cardsWithMoveAbilities,
                    sensorsWithMoveAbilities,
                    cardsWithLocationEffectBlocks,
                    blockedEffectsByColumn,
                    cardsWithCardEffectBlocks,
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
            var cardsWithLocationEffectBlocks = game
                .AllCards.Where(c => c.Ongoing is OngoingBlockLocationEffect<ICard>)
                .ToList();

            // Every entry in this list is a set of cards we can afford to play
            var playableCardSets = GetPlayableCardSets(game[side]);

            // This is the count of open slots by column,
            // and also where we check if PlayCards is blocked
            var availableColumns = GetPlayableCardSlots(game, side, cardsWithLocationEffectBlocks);

            var totalAvailableSlots =
                availableColumns.Left + availableColumns.Middle + availableColumns.Right;

            // Ignore card sets with too many cards (note than an empty set is always present)
            playableCardSets = playableCardSets.Where(s => s.Count <= totalAvailableSlots).ToList();

            var columnChoicesByCount = new Dictionary<int, IReadOnlyList<IReadOnlyList<Column>>>();

            foreach (var cardSet in playableCardSets)
            {
                if (cardSet.Count == 0)
                {
                    yield return new List<IPlayerAction>();
                    continue;
                }

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

            if (total == 0)
            {
                results.Add(new List<Column>());
                return results;
            }

            // This whole block is kind of a stupid optimization, but as far as I can
            // tell from profiling, it actually does work quite well compared to the
            // stack-based one below for small counts.
            //
            // Basically we just try every single possible combination of Left, Right, and Center,
            // i.e. 3^total possibilities, and double-check that it doesn't go over the allowed
            // number for each column. If not, return it. Since we only allocate a new list
            // when we know it's going to return, I think it's basically just CPU-bound,
            // whereas the stack-based solution has a lot of additional memory allocations
            // at each step.
            //
            // I'm still pretty suspicious of it, so it might be worth another look.
            if (total < 4)
            {
                var count = 3;
                for (var i = 1; i < total; i++)
                {
                    count *= 3;
                }

                var result = new List<Column>();

                for (var possibility = 0; possibility < count; possibility++)
                {
                    var valid = true;
                    var current = possibility;
                    result.Clear();

                    var left = 0;
                    var middle = 0;
                    var right = 0;

                    for (var i = 0; i < total; i++)
                    {
                        switch (current % 3)
                        {
                            case 0:
                                result.Add(Column.Left);
                                left++;

                                if (left > available.Left)
                                {
                                    valid = false;
                                }
                                break;
                            case 1:
                                result.Add(Column.Middle);
                                middle++;
                                if (middle > available.Middle)
                                {
                                    valid = false;
                                }
                                break;
                            case 2:
                                result.Add(Column.Right);
                                right++;
                                if (right > available.Right)
                                {
                                    valid = false;
                                }
                                break;
                            default:
                                throw new NotImplementedException();
                        }

                        if (!valid)
                        {
                            break;
                        }

                        current = current / 3;
                    }

                    if (valid)
                    {
                        results.Add(result.ToList());
                    }
                }

                return results;
            }

            // This is a more sane implementation than the above one,
            // but apparently on account of doing so many more allocations
            // or something, it's comparatively slow.
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
        public static (int Left, int Middle, int Right) GetPlayableCardSlots(
            Game game,
            Side side,
            IReadOnlyList<ICard> cardsWithLocationEffectBlocks
        )
        {
            return (
                GetPlayableCardSlots(game, side, Column.Left, cardsWithLocationEffectBlocks),
                GetPlayableCardSlots(game, side, Column.Middle, cardsWithLocationEffectBlocks),
                GetPlayableCardSlots(game, side, Column.Right, cardsWithLocationEffectBlocks)
            );
        }

        public static int GetPlayableCardSlots(
            Game game,
            Side side,
            Column column,
            IReadOnlyList<ICard> cardsWithLocationEffectBlocks
        )
        {
            var blockedEffects = game.GetBlockedEffects(
                column,
                side,
                cardsWithLocationEffectBlocks
            );

            if (blockedEffects.Contains(EffectType.PlayCard))
            {
                return 0;
            }
            else
            {
                return Max.CardsPerLocation - game[column][side].Count;
            }
        }

        /// <summary>
        /// Gets all distinct combinations of cards that can be played out of the given <see cref="Player"/>'s hand.
        ///
        /// Order does not matter.
        ///
        /// Public for testing purposes.
        /// </summary>
        public static IReadOnlyList<IReadOnlyList<ICardInstance>> GetPlayableCardSets(
            IPlayerAccessor player
        )
        {
            return GetPlayableCardSets(player.Energy, player.Hand.ToList());
        }

        private static IReadOnlyList<IReadOnlyList<ICardInstance>> GetPlayableCardSets(
            int energy,
            IReadOnlyList<ICardInstance> hand
        )
        {
            var results = new List<IReadOnlyList<ICardInstance>>();

            // Can always just play no cards.
            results.Add(new List<ICardInstance>());

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
