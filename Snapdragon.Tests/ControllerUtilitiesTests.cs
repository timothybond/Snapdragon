using Snapdragon.PlayerActions;

namespace Snapdragon.Tests
{
    public class ControllerUtilitiesTests
    {
        [Test]
        public async Task GetPossibleMoveActionSets_ComplexMove()
        {
            // This is a test case to examine a bug I ran into with move actions.
            // (It ended up being pretty simple - I was calling ToList on a Stack
            // and needed to reverse it first.)

            // In any event, MoveActions throw an error if they can't be applied,
            // so this test will fail if this situation ends up producing
            // any sets of move actions that are incoherent.
            var game = await TestHelpers
                .PlayCards(Side.Top, Column.Middle, "Squirrel Girl")
                .PlayCards(Side.Top, Column.Right, "Misty Knight")
                .PlayCards(Side.Top, Column.Right, "Cable")
                .PlayCards(new[] { ("Cloak", Column.Left) }, new[] { ("Cloak", Column.Middle) });

            game = await game.StartNextTurn();

            var moveSets = ControllerUtilities.GetPossibleActionSets(game, Side.Top);

            var moveSetsByCount = new List<List<IReadOnlyList<IPlayerAction>>>();

            for (var i = 0; i <= 6; i++)
            {
                moveSetsByCount.Add(moveSets.Where(ms => ms.Count == i).ToList());
            }

            foreach (var moveSet in moveSets)
            {
                var gameWithMoves = moveSet.Aggregate(game, (g, m) => m.Apply(g));
            }

            // TODO: Further test whether this gives the expected outcome(s).
            // Top player has five cards:
            // Left: Squirrel, Cloak
            // Middle: Squirrel Girl
            // Right: Squirrel, Misty Knight, Cable
            //
            // Thanks to both players playing Cloak, the top player can move any
            // card to Left or Middle, giving the following moves:
            //
            // - Squirrel Girl to Left (or no move, 2 possibilities)
            // - Left Squirrel to Middle (or no move - 2 possibilities for one Squirrel)
            // - Right Squirrel to Middle or Left (or no move - 3 possibilities for the other Squirrel)
            // - Cable to Middle or Left (or no move - 3 possibilities for Cable)
            // - Cloak to Middle (or no move - 2 possibilities for Cloak)
            // - Misty Knight to Middle or Left (or no move - 3 possibilities for Misty Knight)
            //
            // So there are 2 * 2 * 3 * 3 * 2 * 3 = 216 possibilities, except that some are invalid.
            //
            // Invalid possibilies:
            // - For three moves:
            //   - Move any 3 possible items to the left while not moving anything from the left (4)
            //
            // - For four moves:
            //   - Move all 4 possible items to the left without moving anything from the left (1)
            //   - Move any 4 possible items to the middle without moving Squirrel Girl from the middle (5)
            //   - Move any 3 possible items to the left while moving a non-left item to the middle (3)
            //
            // - For five moves:
            //   - Move all 4 possible items to the left while moving only one item from the left (2)
            //   - Move all 5 possible items to the middle, with no other moves (1)
            //   - Move both left items to the middle and 2/3 of the right items, and also the third right item to the left (3)
            //
            // - For six moves:
            //   - Move all 5 possible items to the middle, while moving Squirrel Girl to the left (1)
            //
            // The total number of invalid moves is therefore 4 + 1 + 5 + 3 + 2 + 1 + 3 + 1 = 20
            // So we have 216 - 20 = 196 valid moves
            Assert.That(moveSets.Count, Is.EqualTo(196));

            // It would be really painful to write out the derivation of the following, so I have not.
            Assert.That(moveSetsByCount[0].Count, Is.EqualTo(1));
            Assert.That(moveSetsByCount[1].Count, Is.EqualTo(9));
            Assert.That(moveSetsByCount[2].Count, Is.EqualTo(33));
            Assert.That(moveSetsByCount[3].Count, Is.EqualTo(59));
            Assert.That(moveSetsByCount[4].Count, Is.EqualTo(57));
            Assert.That(moveSetsByCount[5].Count, Is.EqualTo(30));
            Assert.That(moveSetsByCount[6].Count, Is.EqualTo(7));
        }

        [Test]
        [TestCase(Side.Top)]
        [TestCase(Side.Bottom)]
        public async Task GetPossibleActionSets_GetsExpectedValues(Side side)
        {
            var oneOne = new Card(Cards.OneOne, Side.Top, CardState.InHand);
            var oneTwo = new Card(Cards.OneTwo, Side.Top, CardState.InHand);

            var game = TestHelpers.NewGame();
            var player = game[side];

            game = game.WithPlayer(player with { Hand = player.Hand.Add(oneOne).Add(oneTwo) });

            // Need to make sure we have 2 energy
            game = await (await game.PlaySingleTurn()).PlaySingleTurn();

            var possibleActionSets = ControllerUtilities.GetPossibleActionSets(game, side);

            // We can:
            // - Do nothing (1)
            // - Play one card in any of three locations (3)
            // - Play the other card in any of three locations (3)
            // - Play both cards, either in any of three locations (9)
            Assert.That(possibleActionSets.Count, Is.EqualTo(16));

            // "Do nothing" exists
            Assert.That(possibleActionSets.Count(s => s.Count == 0), Is.EqualTo(1));

            var singleCardPlays = possibleActionSets.Where(s => s.Count == 1).ToList();
            Assert.That(singleCardPlays.Count, Is.EqualTo(6));

            // Playing the first card only
            var playOneOne = singleCardPlays
                .Where(s =>
                    s[0] is PlayCardAction playAction
                    && string.Equals(playAction.Card.Name, Cards.OneOne.Name)
                )
                .ToList();

            Assert.That(playOneOne.Count, Is.EqualTo(3));

            Assert.That(
                playOneOne.Any(s =>
                    s[0] is PlayCardAction playAction && playAction.Column == Column.Left
                )
            );
            Assert.That(
                playOneOne.Any(s =>
                    s[0] is PlayCardAction playAction && playAction.Column == Column.Middle
                )
            );
            Assert.That(
                playOneOne.Any(s =>
                    s[0] is PlayCardAction playAction && playAction.Column == Column.Right
                )
            );

            // Playing the second card only
            var playOneTwo = singleCardPlays
                .Where(s =>
                    s[0] is PlayCardAction playAction
                    && string.Equals(playAction.Card.Name, Cards.OneOne.Name)
                )
                .ToList();

            Assert.That(playOneTwo.Count, Is.EqualTo(3));

            Assert.That(
                playOneTwo.Any(s =>
                    s[0] is PlayCardAction playAction && playAction.Column == Column.Left
                )
            );
            Assert.That(
                playOneTwo.Any(s =>
                    s[0] is PlayCardAction playAction && playAction.Column == Column.Middle
                )
            );
            Assert.That(
                playOneTwo.Any(s =>
                    s[0] is PlayCardAction playAction && playAction.Column == Column.Right
                )
            );

            // Playing both cards, all permutations
            var playBothCards = possibleActionSets.Where(s => s.Count == 2).ToList();

            Assert.That(playBothCards.Count, Is.EqualTo(9));

            // All sets must play both cards
            Assert.That(
                playBothCards.All(s =>
                    s.Cast<PlayCardAction>().Any(p => string.Equals(p.Card.Name, Cards.OneOne.Name))
                )
            );
            Assert.That(
                playBothCards.All(s =>
                    s.Cast<PlayCardAction>().Any(p => string.Equals(p.Card.Name, Cards.OneTwo.Name))
                )
            );

            // Transform them into the chosen columns (ordered by card power)
            // to check that all nine are present
            var columnChoiceSets = playBothCards
                .Select(s =>
                    s.Cast<PlayCardAction>()
                        .OrderBy(p => p.Card.Power)
                        .Select(p => p.Column)
                        .ToList()
                )
                .ToList();

            AssertHasColumns(columnChoiceSets, Column.Left, Column.Left);
            AssertHasColumns(columnChoiceSets, Column.Left, Column.Middle);
            AssertHasColumns(columnChoiceSets, Column.Left, Column.Right);
            AssertHasColumns(columnChoiceSets, Column.Middle, Column.Left);
            AssertHasColumns(columnChoiceSets, Column.Middle, Column.Middle);
            AssertHasColumns(columnChoiceSets, Column.Middle, Column.Right);
            AssertHasColumns(columnChoiceSets, Column.Right, Column.Left);
            AssertHasColumns(columnChoiceSets, Column.Right, Column.Middle);
            AssertHasColumns(columnChoiceSets, Column.Right, Column.Right);
        }

        private static void AssertHasColumns(
            IReadOnlyList<IReadOnlyList<Column>> columnSets,
            params Column[] columns
        )
        {
            Assert.That(columnSets.Any(cols => cols.SequenceEqual(columns)));
        }

        [Test]
        public void GetPlayableCards()
        {
            var oneOne = new Card(Cards.OneOne, Side.Top, CardState.InHand);
            var oneTwo = new Card(Cards.OneTwo, Side.Top, CardState.InHand);
            var twoOne = new Card(Cards.TwoOne, Side.Top, CardState.InHand);
            var twoTwo = new Card(Cards.TwoTwo, Side.Top, CardState.InHand);

            var controller = new RandomPlayerController();
            var player = new Player(
                new PlayerConfiguration("Test", new Deck([], Guid.Empty), controller),
                Side.Top,
                4,
                new Library([]),
                [oneOne, oneTwo, twoOne, twoTwo],
                [],
                []
            );

            var playableCardSets = ControllerUtilities.GetPlayableCardSets(player);

            // Results should include (13 total):
            // []
            // [OneOne],
            // [OneOne, OneTwo]
            // [OneOne, OneTwo, TwoOne]
            // [OneOne, OneTwo, TwoTwo]
            // [OneOne, TwoOne]
            // [OneOne, TwoTwo]
            // [OneTwo],
            // [OneTwo, TwoOne]
            // [OneTwo, TwoTwo]
            // [TwoOne]
            // [TwoOne, TwoTwo]
            // [TwoTwo]
            Assert.That(playableCardSets.Count, Is.EqualTo(13));

            var emptyCards = playableCardSets.Where(s => s.Count == 0).ToList();
            Assert.That(emptyCards.Count, Is.EqualTo(1));

            var singleCards = playableCardSets.Where(s => s.Count == 1).ToList();
            Assert.That(singleCards.Count, Is.EqualTo(4));

            Assert.That(singleCards.Any(s => s.Contains(oneOne)));
            Assert.That(singleCards.Any(s => s.Contains(oneTwo)));
            Assert.That(singleCards.Any(s => s.Contains(twoOne)));
            Assert.That(singleCards.Any(s => s.Contains(twoTwo)));

            // [OneOne, OneTwo]
            // [OneOne, TwoOne]
            // [OneOne, TwoTwo]
            // [OneTwo, TwoOne]
            // [OneTwo, TwoTwo]
            // [TwoOne, TwoTwo]
            var doubleCards = playableCardSets.Where(s => s.Count == 2).ToList();
            Assert.That(doubleCards.Count, Is.EqualTo(6));
            Assert.That(doubleCards.Any(s => s.Contains(oneOne) && s.Contains(oneTwo)));
            Assert.That(doubleCards.Any(s => s.Contains(oneOne) && s.Contains(twoOne)));
            Assert.That(doubleCards.Any(s => s.Contains(oneOne) && s.Contains(twoTwo)));
            Assert.That(doubleCards.Any(s => s.Contains(oneTwo) && s.Contains(twoOne)));
            Assert.That(doubleCards.Any(s => s.Contains(oneTwo) && s.Contains(twoTwo)));
            Assert.That(doubleCards.Any(s => s.Contains(twoOne) && s.Contains(twoTwo)));

            // [OneOne, OneTwo, TwoOne]
            // [OneOne, OneTwo, TwoTwo]
            var tripleCards = playableCardSets.Where(s => s.Count == 3).ToList();
            Assert.That(tripleCards.Count, Is.EqualTo(2));

            Assert.That(tripleCards.All(s => s.Contains(oneOne)));
            Assert.That(tripleCards.All(s => s.Contains(oneTwo)));
            Assert.That(tripleCards.Any(s => s.Contains(twoOne)));
            Assert.That(tripleCards.Any(s => s.Contains(twoTwo)));
        }

        [Test]
        public void GetColumnChoices_OneOfEach()
        {
            var allChoices = ControllerUtilities.GetPossibleColumnChoices(3, (1, 1, 1));

            // Six choices
            Assert.That(allChoices.Count, Is.EqualTo(6));

            AssertHas(allChoices, Column.Left, Column.Middle, Column.Right);
            AssertHas(allChoices, Column.Left, Column.Right, Column.Middle);
            AssertHas(allChoices, Column.Middle, Column.Left, Column.Right);
            AssertHas(allChoices, Column.Middle, Column.Right, Column.Left);
            AssertHas(allChoices, Column.Right, Column.Left, Column.Middle);
            AssertHas(allChoices, Column.Right, Column.Middle, Column.Left);
        }

        [Test]
        public void GetColumnChoices_TwoLeftTwoMiddleFullRight()
        {
            var choices = ControllerUtilities.GetPossibleColumnChoices(4, (2, 2, 0));

            // Four-choose-two has six possible outcomes
            Assert.That(choices.Count, Is.EqualTo(6));

            // All outcomes must have two Lefts and two Middles
            Assert.That(choices.All(choice => choice.Count(col => col == Column.Left) == 2));
            Assert.That(choices.All(choice => choice.Count(col => col == Column.Middle) == 2));

            // No two outcomes should match
            for (var i = 0; i < choices.Count - 1; i++)
            {
                for (var j = i + 1; j < choices.Count; j++)
                {
                    Assert.That(choices[i].SequenceEqual(choices[j]), Is.False);
                }
            }
        }

        private void AssertHas(
            IReadOnlyList<IReadOnlyList<Column>> choices,
            params Column[] sequence
        )
        {
            Assert.That(choices.Any(c => c.SequenceEqual(sequence)));
        }
    }
}
