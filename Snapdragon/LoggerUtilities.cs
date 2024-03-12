using System.Text;

namespace Snapdragon
{
    public static class LoggerUtilities
    {
        public static string HandsLog(Game game)
        {
            var builder = new StringBuilder();

            builder.Append("Top Hand: [");
            builder.Append(string.Join(", ", game.Top.Hand.Select(c => $"{c.Name} ({c.Id})")));
            builder.AppendLine("]");

            builder.Append("Bottom Hand: [");
            builder.Append(string.Join(", ", game.Bottom.Hand.Select(c => $"{c.Name} ({c.Id})")));
            builder.AppendLine("]");

            return builder.ToString();
        }

        public static string GameStateLog(Game game)
        {
            var columnWidths = new Dictionary<Column, int>
            {
                { Column.Left, GetWidth(game.Left) },
                { Column.Middle, GetWidth(game.Middle) },
                { Column.Right, GetWidth(game.Right) }
            };

            var scores = game.GetCurrentScores();

            var builder = new StringBuilder();
            builder.AppendLine(string.Empty.PadLeft(columnWidths.Values.Sum() + 7, '-'));

            builder.WriteSide(game, Side.Top, columnWidths, scores);
            builder.WriteSide(game, Side.Bottom, columnWidths, scores);

            return builder.ToString();
        }

        private static void WriteSide(
            this StringBuilder builder,
            Game game,
            Side side,
            IReadOnlyDictionary<Column, int> columnWidths,
            CurrentScores scores
        )
        {
            for (var i = 0; i < Max.CardsPerLocation; i++)
            {
                foreach (var location in new[] { game.Left, game.Middle, game.Right })
                {
                    builder.Append("| ");

                    var cards = location[side];
                    var name = cards.Count <= i ? "" : cards[i].Name;
                    var power = cards.Count <= i ? "" : $"({cards[i].AdjustedPower})";
                    builder.Append($"{name} {power}".PadRight(columnWidths[location.Column]));
                }

                builder.Append("|");
                builder.AppendLine();
            }

            foreach (var column in new[] { Column.Left, Column.Middle, Column.Right })
            {
                var totalPower = scores[column][side];
                var winner = scores[column].Leader == side;

                var scoreString = totalPower.ToString();
                if (winner)
                {
                    scoreString = $" [{scoreString}] ";
                }
                else
                {
                    scoreString = $"  {scoreString}  ";
                }

                scoreString = scoreString.PadLeft(
                    (columnWidths[column] / 2) + (scoreString.Length / 2),
                    '-'
                );
                scoreString = scoreString.PadRight(columnWidths[column], '-');
                builder.Append("|-");
                builder.Append(scoreString);
            }

            builder.Append("|");
            builder.AppendLine();
        }

        private static int GetWidth(Location location)
        {
            if (location.AllCards.Count() == 0)
            {
                // Reasonable minimum
                return 12;
            }

            // One space on either side,
            // one space before the power,
            // two parentheses,
            // two digit power
            return location.AllCards.Select(c => c.Name.Length).Max() + 7;
        }
    }
}
