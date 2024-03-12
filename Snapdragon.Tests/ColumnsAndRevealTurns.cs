using System.Collections;

namespace Snapdragon.Tests
{
    /// <summary>
    /// Helper type for tests to run for each column, and also pass the turn that <see cref="Location"/> is revealed.
    /// </summary>
    public class ColumnsAndRevealTurns : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            yield return new object[] { Column.Left, 1 };
            yield return new object[] { Column.Middle, 2 };
            yield return new object[] { Column.Right, 3 };
        }
    }
}
