using System.Text;

namespace Snapdragon.Tests
{
    public class TestLogger : IGameLogger
    {
        private StringBuilder output = new StringBuilder();

        public Task LogEvent(Event e)
        {
            Console.WriteLine(e.ToString());
            output.AppendLine(e.ToString());

            return Task.CompletedTask;
        }

        public Task LogFinishedGame(Game game)
        {
            // Do nothing - we already write detailed logs throughout the game with this implementation.
            return Task.CompletedTask;
        }

        public Task LogGameState(Game game)
        {
            output.AppendLine();
            output.AppendLine(LoggerUtilities.GameStateLog(game));
            output.AppendLine();

            Console.WriteLine();
            Console.WriteLine(LoggerUtilities.GameStateLog(game));
            Console.WriteLine();

            return Task.CompletedTask;
        }

        public Task LogHands(Game game)
        {
            output.AppendLine();
            output.AppendLine(LoggerUtilities.HandsLog(game));
            output.AppendLine();

            Console.WriteLine();
            Console.WriteLine(LoggerUtilities.HandsLog(game));
            Console.WriteLine();

            return Task.CompletedTask;
        }

        public override string ToString()
        {
            return output.ToString();
        }
    }
}
