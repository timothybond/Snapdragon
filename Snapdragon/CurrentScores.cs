using System.Data.Common;

namespace Snapdragon
{
    public record CurrentScores(LocationScores Left, LocationScores Middle, LocationScores Right)
    {
        public CurrentScores()
            : this(
                new LocationScores(Column.Left, 0, 0),
                new LocationScores(Column.Middle, 0, 0),
                new LocationScores(Column.Right, 0, 0)
            ) { }

        public LocationScores this[Column column]
        {
            get
            {
                switch (column)
                {
                    case Column.Left:
                        return this.Left;
                    case Column.Middle:
                        return this.Middle;
                    case Column.Right:
                        return this.Right;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        public CurrentScores WithLocationScores(LocationScores locationScores)
        {
            switch (locationScores.Column)
            {
                case Column.Left:
                    return this with { Left = locationScores };
                case Column.Middle:
                    return this with { Middle = locationScores };
                case Column.Right:
                    return this with { Right = locationScores };
                default:
                    throw new NotImplementedException();
            }
        }

        public CurrentScores WithAddedPower(int amount, Column column, Side side)
        {
            return this.WithLocationScores(this[column].WithAddedPower(amount, side));
        }

        public Side? Leader
        {
            get
            {
                var topCount = 0;
                var bottomCount = 0;

                var topTotal = 0;
                var bottomTotal = 0;

                foreach (var column in All.Columns)
                {
                    var scores = this[column];
                    if (scores.Leader == Side.Top)
                    {
                        topCount += 1;
                    }
                    else if (scores.Leader == Side.Bottom)
                    {
                        bottomCount += 1;
                    }

                    topTotal += scores.Top;
                    bottomTotal += scores.Bottom;
                }

                if (topCount > bottomCount)
                {
                    return Side.Top;
                }
                else if (topCount < bottomCount)
                {
                    return Side.Bottom;
                }
                else if (topTotal > bottomTotal)
                {
                    return Side.Top;
                }
                else if (topTotal < bottomTotal)
                {
                    return Side.Bottom;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
