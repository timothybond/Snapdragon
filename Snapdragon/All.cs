using System.Collections.Immutable;

namespace Snapdragon
{
    /// <summary>
    /// Helper class to quickly reference possible values of enums.
    /// </summary>
    public static class All
    {
        public static readonly ImmutableList<Column> Columns =
        [
            Column.Left,
            Column.Middle,
            Column.Right
        ];

        public static readonly ImmutableList<Side> Sides = [Side.Top, Side.Bottom];
    }
}
