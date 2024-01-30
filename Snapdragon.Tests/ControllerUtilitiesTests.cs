namespace Snapdragon.Tests
{
    public class ControllerUtilitiesTests
    {
        [Test]
        public void GetPlayableCards()
        {
            var oneOne = new Card(Cards.OneOne, Side.Top, CardState.InHand);
            var oneTwo = new Card(Cards.OneTwo, Side.Top, CardState.InHand);
            var twoOne = new Card(Cards.TwoOne, Side.Top, CardState.InHand);
            var twoTwo = new Card(Cards.TwoTwo, Side.Top, CardState.InHand);

            var controller = new RandomPlayerController();
            var player = new Player(
                new PlayerConfiguration("Test", new Deck([]), controller),
                Side.Top,
                4,
                new Library([]),
                [oneOne, oneTwo, twoOne, twoTwo]
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
