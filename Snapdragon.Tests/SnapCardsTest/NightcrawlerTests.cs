using Snapdragon.PlayerActions;

namespace Snapdragon.Tests.SnapCardsTest
{
    public class NightcrawlerTests
    {
        [Test]
        [TestCaseSource(typeof(AllSidesAndDifferentColumns))]
        public async Task HasNotMoved_CanMove(Side side, Column column, Column otherColumn)
        {
            var game = await TestHelpers.PlayCards(side, column, "Nightcrawler");

            Assert.That(game[column][side].Count, Is.EqualTo(1));
            var nightcrawler = game[column][side][0];

            Assert.That(nightcrawler.Name, Is.EqualTo("Nightcrawler"));

            Assert.That(game.CanMove(nightcrawler, otherColumn));
            Assert.That(game.CanMove(nightcrawler, otherColumn), Is.True);
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndThreeDifferentColumns))]
        public async Task HasMoved_CanNotMove(
            Side side,
            Column initial,
            Column moveTo,
            Column remainingColumn
        )
        {
            var game = await TestHelpers.PlayCards(side, initial, "Nightcrawler");
            var nightcrawler = game[initial][side][0];

            var controller = (TestPlayerController)game[side].Controller;
            controller.Actions = new List<IPlayerAction>
            {
                new MoveCardAction(side, nightcrawler, initial, moveTo)
            };

            game = await game.PlaySingleTurn();

            nightcrawler = game[moveTo][side][0];
            Assert.That(nightcrawler.Name, Is.EqualTo("Nightcrawler"));

            Assert.That(game.CanMove(nightcrawler, initial), Is.False);
            Assert.That(game.CanMove(nightcrawler, remainingColumn), Is.False);
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndDifferentColumns))]
        public async Task WhenMoved_TransitionsColumns(Side side, Column initial, Column moveTo)
        {
            var game = await TestHelpers.PlayCards(side, initial, "Nightcrawler");
            var nightcrawler = game[initial][side][0];

            var controller = (TestPlayerController)game[side].Controller;
            controller.Actions = new List<IPlayerAction>
            {
                new MoveCardAction(side, nightcrawler, initial, moveTo)
            };

            game = await game.PlaySingleTurn();

            Assert.That(game[initial][side].Count, Is.EqualTo(0));
            Assert.That(game[moveTo][side].Count, Is.EqualTo(1));

            nightcrawler = game[moveTo][side][0];
            Assert.That(nightcrawler.Name, Is.EqualTo("Nightcrawler"));
        }
    }
}
