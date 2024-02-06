namespace Snapdragon.Tests
{
    /// <summary>
    /// These tests are just to ensure I didn't accidentally make a malformed TestCaseSource.
    /// </summary>
    public class TestCaseSourceTests
    {
        [Test]
        [TestCaseSource(typeof(AllSidesAndDifferentColumns))]
        public void DifferentColumns_ColumnsDoNotMatch(Side side, Column column, Column otherColumn)
        {
            Assert.That(column, Is.Not.EqualTo(otherColumn));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndThreeDifferentColumns))]
        public void ThreeDifferentColumns_ColumnsDoNotMatch(
            Side side,
            Column first,
            Column second,
            Column third
        )
        {
            Assert.That(first, Is.Not.EqualTo(second));
            Assert.That(first, Is.Not.EqualTo(third));
            Assert.That(second, Is.Not.EqualTo(third));
        }
    }
}
