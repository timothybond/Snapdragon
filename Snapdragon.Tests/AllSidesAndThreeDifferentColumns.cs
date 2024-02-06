using System.Collections;

namespace Snapdragon.Tests
{
    /// <summary>
    /// Helper type for tests to run at every combination of the values of
    /// <see cref="Side"/> and two different <see cref="Column"/>s.
    /// </summary>
    public class AllSidesAndThreeDifferentColumns : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            yield return new object[] { Side.Top, Column.Left, Column.Middle, Column.Right };
            yield return new object[] { Side.Top, Column.Left, Column.Right, Column.Middle };
            yield return new object[] { Side.Top, Column.Middle, Column.Left, Column.Right };
            yield return new object[] { Side.Top, Column.Middle, Column.Right, Column.Left };
            yield return new object[] { Side.Top, Column.Right, Column.Left, Column.Middle };
            yield return new object[] { Side.Top, Column.Right, Column.Middle, Column.Left };
            yield return new object[] { Side.Bottom, Column.Left, Column.Middle, Column.Right };
            yield return new object[] { Side.Bottom, Column.Left, Column.Right, Column.Middle };
            yield return new object[] { Side.Bottom, Column.Middle, Column.Left, Column.Right };
            yield return new object[] { Side.Bottom, Column.Middle, Column.Right, Column.Left };
            yield return new object[] { Side.Bottom, Column.Right, Column.Left, Column.Middle };
            yield return new object[] { Side.Bottom, Column.Right, Column.Middle, Column.Left };
        }
    }
}
