using System.Collections.Immutable;

namespace Snapdragon
{
    public enum Column
    {
        Left = 0,
        Middle = 1,
        Right = 2
    }

    public static class ColumnExtensions
    {
        private static readonly ImmutableDictionary<Column, ImmutableList<Column>> otherColumns =
            new Dictionary<Column, ImmutableList<Column>>
            {
                { Column.Left, [Column.Middle, Column.Right] },
                { Column.Middle, [Column.Left, Column.Right] },
                { Column.Right, [Column.Left, Column.Middle] }
            }.ToImmutableDictionary();

        public static ImmutableList<Column> Others(this Column column)
        {
            return otherColumns[column];
        }
    }
}
