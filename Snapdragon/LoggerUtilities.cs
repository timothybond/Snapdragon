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

            foreach (var location in game.Locations)
            {
                builder.Append("-");
                var totalPadding =
                    columnWidths[location.Column] - location.Definition.Name.Length - 4;

                for (var i = 0; i < totalPadding / 2; i++)
                {
                    builder.Append("-");
                }
                builder.Append("[");
                builder.Append(location.Revealed ? " " : "(");
                builder.Append(location.Definition.Name);
                builder.Append(location.Revealed ? " " : ")");
                builder.Append("]");

                for (var i = 0; i < totalPadding - totalPadding / 2; i++)
                {
                    builder.Append("-");
                }
                builder.Append("-");
            }
            builder.AppendLine("-");

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
                return location.Definition.Name.Length + 4;
            }

            // One space on either side,
            // one space before the power,
            // two parentheses,
            // two digit power
            return Math.Max(
                location.AllCards.Select(c => c.Name.Length).Max() + 7,
                location.Definition.Name.Length + 4
            );
        }
    }
}
