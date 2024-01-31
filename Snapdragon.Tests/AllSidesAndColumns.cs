using System.Collections;

namespace Snapdragon.Tests
{
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
