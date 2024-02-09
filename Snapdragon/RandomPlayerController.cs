using Snapdragon.OngoingAbilities;
using Snapdragon.PlayerActions;
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
            // This function implementation basically follows the logic in
            // ControllerUtilities.GetPossibleActionSets, except that it
            // stops at each point to pick one choice, to avoid enumerating
            // a bunch of possibilities we don't care about.

            var cardsWithMoveAbilities = new List<Card>();
            var cardsWithLocationEffectBlocks = new List<Card>();

            foreach (var card in game.AllCards)
            {
                if (card.MoveAbility != null)
                {
                    cardsWithMoveAbilities.Add(card);
                }

                if (card.Ongoing != null && card.Ongoing is OngoingBlockLocationEffect<Card>)
                {
                    cardsWithLocationEffectBlocks.Add(card);
                }
            }

            // First we do moves. Note this means that when a lot of move actions are provided,
            // there is a pretty low chance we go with "no moves". I think this is probably
            // sensible on the premise that generally there will only BE a lot of moves
            // on offer if we're playing a move deck, although Cloak does provide move options
            // to both sides, so maybe sometimes this is a bad assumption.
            var moves = Random.Of(
                GetPossibleMoveActionSets(
                    game,
                    side,
                    cardsWithMoveAbilities,
                    cardsWithLocationEffectBlocks
                )
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
                if (card.Ongoing != null && card.Ongoing is OngoingBlockLocationEffect<Card>)
                {
                    cardsWithLocationEffectBlocks.Add(card);
                }
            }

            var playableCardSets = GetPlayableCardSets(game[side]);

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
                return moves.Concat(Random.Of(nonBlockedActionSets)).ToList();
            }

            // Apparently there were no valid places to play these cards.
            // This should be a fairly-uncommon occurrence, so for now I'm just going to
            // set it to play whatever cards it can from the original random columns above.
            var remainingValidPlays = cardsToPlay
                .Select((c, i) => new PlayCardAction(side, c, randomColumnChoices[i]))
                .Where(a => !IsBlocked(a, game));

            return moves.Concat(remainingValidPlays).ToList();
        }

        private static IReadOnlyList<PlayCardAction> GetPlayCardActions(
            IReadOnlyList<Card> cards,
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
