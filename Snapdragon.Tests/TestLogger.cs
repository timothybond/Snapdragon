using System.Text;

namespace Snapdragon.Tests
{
    public class TestLogger : IGameLogger
    {
        private StringBuilder output = new StringBuilder();

        public void LogEvent(Event e)
        {
            Console.WriteLine(e.ToString());
            output.AppendLine(e.ToString());
        }

        public void LogGameState(Game game)
        {
            output.AppendLine();
            output.AppendLine(LoggerUtilities.GameStateLog(game));
            output.AppendLine();

            Console.WriteLine();
            Console.WriteLine(LoggerUtilities.GameStateLog(game));
            Console.WriteLine();
        }

        public void LogHands(Game game)
        {
            output.AppendLine();
            output.AppendLine(LoggerUtilities.HandsLog(game));
            output.AppendLine();

            Console.WriteLine();
            Console.WriteLine(LoggerUtilities.HandsLog(game));
            Console.WriteLine();
        }

        public override string ToString()
        {
            return output.ToString();
        }
    }
}
