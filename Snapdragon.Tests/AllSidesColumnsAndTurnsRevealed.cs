using System.Collections;

namespace Snapdragon.Tests
{
    /// <summary>
    /// Helper type for tests to run at every combination of the values of
    /// <see cref="Side"/>, <see cref="Column"/>, and Turn, but only for
    /// those turns where the <see cref="Location"/> at that <see cref="Column"/>
    /// is revealed.
    /// </summary>
    public class AllSidesColumnsAndTurnsRevealed : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            foreach (var side in All.Sides)
            {
                foreach (var column in All.Columns)
                {
                    for (var turn = 1; turn <= 6; turn++)
                    {
                        if (All.Columns.IndexOf(column) + 1 <= turn)
                        {
                            yield return new object[] { side, column, turn };
                        }
                    }
                }
            }
        }
    }
}
