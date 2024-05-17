using Snapdragon.Fluent.Ongoing;
using Snapdragon.PlayerActions;
using static Snapdragon.ControllerUtilities;

namespace Snapdragon
{
    /// <summary>
    /// An implementation of <see cref="IPlayerController"/> that essentially just makes random moves.
    /// </summary>
    public class RandomPlayerController : IPlayerController
    {
        public override string ToString()
        {
            return "Random";
        }

        public IReadOnlyList<IPlayerAction> GetActions(Game game, Side side)
        {
            // This function implementation basically follows the logic in
            // ControllerUtilities.GetPossibleActionSets, except that it
            // stops at each point to pick one choice, to avoid enumerating
            // a bunch of possibilities we don't care about.

            var cardsWithMoveAbilities = new List<ICard>();
            var cardsWithLocationEffectBlocks = new List<ICard>();
            var cardsWithCardEffectBlocks = new List<ICard>();
            var sensorsWithMoveAbilities = new List<Sensor<ICard>>();
            var locationsWithLocationEffectBlocks = new List<Location>();

            foreach (var card in game.AllCards)
            {
                if (card.MoveAbility != null)
                {
                    cardsWithMoveAbilities.Add(card);
                }

                if (card.Ongoing != null && card.Ongoing is OngoingBlockLocationEffect<ICard>)
                {
                    cardsWithLocationEffectBlocks.Add(card);
                }

                if (card.Ongoing != null && card.Ongoing is OngoingBlockCardEffect<ICard>)
                {
                    cardsWithCardEffectBlocks.Add(card);
                }

                if (card.MoveAbility != null)
                {
                    cardsWithMoveAbilities.Add(card);
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

            var blockedEffectsByColumn = game.GetBlockedEffectsByColumn(
                cardsWithLocationEffectBlocks,
                locationsWithLocationEffectBlocks,
                side
            );

            // First we do moves. Note this means that when a lot of move actions are provided,
            // there is a pretty low chance we go with "no moves". I think this is probably
            // sensible on the premise that generally there will only BE a lot of moves
            // on offer if we're playing a move deck, although Cloak does provide move options
            // to both sides, so maybe sometimes this is a bad assumption.
            var moves = GetRandomMoves(
                game,
                side,
                cardsWithMoveAbilities,
                sensorsWithMoveAbilities,
                cardsWithLocationEffectBlocks,
                blockedEffectsByColumn,
                cardsWithCardEffectBlocks
            );

            // Once we pick the moves, we have to perform the rest of the search
            // as though they have already taken place.
            game = moves.Aggregate(game, (g, m) => m.Apply(g));

            // Unfortunately we need to figure out effect-blocks
            // again in case somebody moves who blocks card play
            // (ran into this right away with Ebony Maw).
            cardsWithLocationEffectBlocks.Clear();

            foreach (var card in game.AllCards)
            {
                if (card.Ongoing?.Type == OngoingAbilityType.BlockLocationEffects)
                {
                    cardsWithLocationEffectBlocks.Add(card);
                }
            }

            var playableCardSets = GetPlayableCardSets(game[side]);

            foreach (var playableCardSet in playableCardSets)
            {
                if (playableCardSet.Sum(c => c.Cost) > 6)
                {
                    throw new InvalidOperationException();
                }
            }

            var availableColumns = GetPlayableCardSlots(game, side, cardsWithLocationEffectBlocks);
            var totalAvailableSlots =
                availableColumns.Left + availableColumns.Middle + availableColumns.Right;

            playableCardSets = playableCardSets.Where(s => s.Count <= totalAvailableSlots).ToList();

            var cardsToPlay = Random.Of(playableCardSets);

            if (cardsToPlay.Count == 0)
            {
                return moves;
            }

            var columnChoiceSets = GetPossibleColumnChoices(cardsToPlay.Count, availableColumns);
            var randomColumnChoices = Random.Of(columnChoiceSets);

            // In most cases this will be a valid choice, because most cards don't have play restrictions.
            // That's also why I'm trying this first instead of looping through them and checking validity.
            if (
                !cardsToPlay
                    .Select(
                        (c, i) =>
                            c.PlayRestriction?.IsBlocked(game, randomColumnChoices[i], c) ?? false
                    )
                    .Any(blocked => blocked)
            )
            {
                return moves
                    .Cast<IPlayerAction>()
                    .Concat(GetPlayCardActions(cardsToPlay, randomColumnChoices, side))
                    .ToList();
            }

            // If there were play restrictions above, try to find a set of column choices that passes.

            var nonBlockedActionSets = columnChoiceSets
                .Select(columns => GetPlayCardActions(cardsToPlay, columns, side))
                .Where(actionSet => !actionSet.Any(a => IsBlocked(a, game)))
                .ToList();

            if (nonBlockedActionSets.Count > 0)
            {
                return moves.Cast<IPlayerAction>().Concat(Random.Of(nonBlockedActionSets)).ToList();
            }

            // Apparently there were no valid places to play these cards.
            // This should be a fairly-uncommon occurrence, so for now I'm just going to
            // set it to play whatever cards it can from the original random columns above.
            var remainingValidPlays = cardsToPlay
                .Select((c, i) => new PlayCardAction(side, c, randomColumnChoices[i]))
                .Where(a => !IsBlocked(a, game));

            return moves.Cast<IPlayerAction>().Concat(remainingValidPlays).ToList();
        }

        private static IReadOnlyList<MoveCardAction> GetRandomMoves(
            Game game,
            Side side,
            IReadOnlyList<ICard> cardsWithMoveAbilities,
            IReadOnlyList<Sensor<ICard>> sensorsWithMoveAbilities,
            IReadOnlyList<ICard> cardsWithLocationEffectBlocks,
            IReadOnlyDictionary<Column, IReadOnlySet<EffectType>> blockedEffectsByColumn,
            IReadOnlyList<ICard> cardsWithCardEffectBlocks
        )
        {
            var moves = new List<MoveCardAction>();

            var moveableCards = GetMoveableCards(
                game,
                side,
                cardsWithMoveAbilities,
                sensorsWithMoveAbilities,
                blockedEffectsByColumn,
                cardsWithCardEffectBlocks
            );

            while (moveableCards.Count > 0)
            {
                var nextIndex = Random.Next(moveableCards.Count);
                var next = moveableCards[nextIndex];
                moveableCards.RemoveAt(nextIndex);

                var potentialColumns = PotentialNewColumns(
                    next,
                    game,
                    cardsWithMoveAbilities,
                    sensorsWithMoveAbilities,
                    cardsWithLocationEffectBlocks,
                    blockedEffectsByColumn,
                    cardsWithCardEffectBlocks
                );

                if (potentialColumns.Count == 0)
                {
                    continue;
                }

                // Note we can always choose not to move a card
                var newColumn = Random.Next(potentialColumns.Count + 1);

                if (newColumn == potentialColumns.Count)
                {
                    continue;
                }

                var newMove = new MoveCardAction(
                    next.Side,
                    next,
                    next.Column,
                    potentialColumns[newColumn]
                );

                game = newMove.Apply(game);

                moves.Add(newMove);
            }

            return moves;
        }

        private static IReadOnlyList<Column> PotentialNewColumns(
            ICard card,
            Game game,
            IReadOnlyList<ICard> cardsWithMoveAbilities,
            IReadOnlyList<Sensor<ICard>> sensorsWithMoveAbilities,
            IReadOnlyList<ICard> cardsWithLocationEffectBlocks,
            IReadOnlyDictionary<Column, IReadOnlySet<EffectType>> blockedEffectsByColumn,
            IReadOnlyList<ICard> cardsWithCardEffectBlocks
        )
        {
            return card
                .Column.Others()
                .Where(col =>
                    game.CanMove(
                        card,
                        col,
                        blockedEffectsByColumn,
                        cardsWithMoveAbilities,
                        sensorsWithMoveAbilities,
                        cardsWithCardEffectBlocks
                    )
                    && game[col][card.Side].Count < Max.CardsPerLocation
                )
                .ToList();
        }

        private static List<ICard> GetMoveableCards(
            Game game,
            Side side,
            IReadOnlyList<ICard> cardsWithMoveAbilities,
            IReadOnlyList<Sensor<ICard>> sensorsWithMoveAbilities,
            IReadOnlyDictionary<Column, IReadOnlySet<EffectType>> blockedEffectsByColumn,
            IReadOnlyList<ICard> cardsWithCardEffectBlocks
        )
        {
            var result = new List<ICard>();

            var allCards = game.Left[side].Concat(game.Middle[side]).Concat(game.Right[side]);

            foreach (var card in allCards)
            {
                foreach (var column in card.Column.Others())
                {
                    if (
                        game.CanMove(
                            card,
                            column,
                            blockedEffectsByColumn,
                            cardsWithMoveAbilities,
                            sensorsWithMoveAbilities,
                            cardsWithCardEffectBlocks
                        )
                    )
                    {
                        result.Add(card);
                    }
                }
            }

            return result;
        }

        private static IReadOnlyList<PlayCardAction> GetPlayCardActions(
            IReadOnlyList<ICardInstance> cards,
            IReadOnlyList<Column> columns,
            Side side
        )
        {
            if (cards.Count != columns.Count)
            {
                throw new ArgumentException(
                    $"GetPlayCardActions was called with {cards.Count} cards but {columns.Count} columns."
                );
            }

            return cards.Select((c, i) => new PlayCardAction(side, c, columns[i])).ToList();
        }

        private static bool IsBlocked(PlayCardAction action, Game game)
        {
            return action.Card.PlayRestriction?.IsBlocked(game, action.Column, action.Card)
                ?? false;
        }
    }
}
