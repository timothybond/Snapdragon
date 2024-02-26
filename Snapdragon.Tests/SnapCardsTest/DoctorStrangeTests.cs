namespace Snapdragon.Tests.SnapCardsTest
{
    public class DoctorStrangeTests
    {
        [Test]
        [TestCaseSource(typeof(AllSidesAndDifferentColumns))]
        public async Task MovesOnlyOtherCard(Side side, Column column, Column otherColumn)
        {
            var game = await TestHelpers
                .PlayCards(side, otherColumn, "Misty Knight")
                .PlayCards(side, column, "Doctor Strange");

            Assert.That(game[column][side].Count, Is.EqualTo(2));

            var mistyKnight = game[column][side][1];
            Assert.That(mistyKnight.Name, Is.EqualTo("Misty Knight"));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndDifferentColumns))]
        public async Task DoesNotMoveEnemyCard(Side side, Column column, Column otherColumn)
        {
            var game = await TestHelpers
                .PlayCards(side.Other(), otherColumn, "Misty Knight")
                .PlayCards(side, column, "Doctor Strange");

            Assert.That(game[column][side.Other()].Count, Is.EqualTo(0));
            Assert.That(game[otherColumn][side.Other()].Count, Is.EqualTo(1));

            var mistyKnight = game[otherColumn][side.Other()][0];
            Assert.That(mistyKnight.Name, Is.EqualTo("Misty Knight"));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndDifferentColumns))]
        public async Task MovesHighestPowerCard(Side side, Column column, Column otherColumn)
        {
            // Note: Hawkeye's ability will trigger, giving him 4 power total
            var game = await TestHelpers
                .PlayCards(side, otherColumn, "Hawkeye")
                .PlayCards(side, otherColumn, "Misty Knight")
                .PlayCards(side, column, "Doctor Strange");

            Assert.That(game[column][side].Count, Is.EqualTo(2));
            Assert.That(game[otherColumn][side].Count, Is.EqualTo(1));

            var hawkeye = game[column][side][1];
            Assert.That(hawkeye.Name, Is.EqualTo("Hawkeye"));

            var mistyKnight = game[otherColumn][side][0];
            Assert.That(mistyKnight.Name, Is.EqualTo("Misty Knight"));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndDifferentColumns))]
        public async Task TieForHighestPower_MovesBothCards(
            Side side,
            Column column,
            Column otherColumn
        )
        {
            // Note: Hawkeye's ability will trigger, giving him 4 power total
            var game = await TestHelpers
                .PlayCards(side, otherColumn, "Hawkeye")
                .PlayCards(side, otherColumn, "Cloak")
                .PlayCards(side, column, "Doctor Strange");

            Assert.That(game[column][side].Count, Is.EqualTo(3));

            // Technically I'm not sure what the canonical order should be,
            // so this test is not going to validate order.
            var otherCardNames = game[column][side].Skip(1).Select(c => c.Name).ToList();

            Assert.Contains("Hawkeye", otherCardNames);
            Assert.Contains("Cloak", otherCardNames);
        }
    }
}
