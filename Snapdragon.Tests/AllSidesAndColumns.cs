using System.Collections;

namespace Snapdragon.Tests
{
    /// <summary>
    /// Helper type for tests to run at every combination of the values of
    /// <see cref="Side"/> and <see cref="Column"/>;
    /// </summary>
    public class AllSidesAndColumns : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            yield return new object[] { Side.Top, Column.Left };
            yield return new object[] { Side.Top, Column.Middle };
            yield return new object[] { Side.Top, Column.Right };
            yield return new object[] { Side.Bottom, Column.Left };
            yield return new object[] { Side.Bottom, Column.Middle };
            yield return new object[] { Side.Bottom, Column.Right };
        }
    }
}
