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
        public static IEnumerable<Column> Others(this Column column)
        {
            switch (column)
            {
                case Column.Left:
                    yield return Column.Middle;
                    yield return Column.Right;
                    break;
                case Column.Middle:
                    yield return Column.Left;
                    yield return Column.Right;
                    break;
                case Column.Right:
                    yield return Column.Left;
                    yield return Column.Middle;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
