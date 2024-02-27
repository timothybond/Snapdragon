namespace Snapdragon.Runner
{
    internal class ConsoleLogger : IGameLogger
    {
        public void LogEvent(Event e)
        {
            Console.WriteLine(e.ToString());
        }

        public void LogGameState(Game game)
        {
            Console.WriteLine(LoggerUtilities.GameStateLog(game));
        }

        public void LogHands(Game game)
        {
            Console.WriteLine(LoggerUtilities.HandsLog(game));
        }

        public Task LogFinishedGame(Game game)
        {
            // Do nothing - we already write detailed logs throughout the game with this implementation.
            return Task.CompletedTask;
        }
    }
}
