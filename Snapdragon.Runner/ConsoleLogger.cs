namespace Snapdragon.Runner
{
    internal class ConsoleLogger : IGameLogger
    {
        public Task LogEvent(Event e)
        {
            Console.WriteLine(e.ToString());
            return Task.CompletedTask;
        }

        public Task LogFinishedGame(Game game)
        {
            // Do nothing - we already write detailed logs throughout the game with this implementation.
            return Task.CompletedTask;
        }

        public Task LogGameState(Game game)
        {
            Console.WriteLine(LoggerUtilities.GameStateLog(game));
            return Task.CompletedTask;
        }

        public Task LogHands(Game game)
        {
            Console.WriteLine(LoggerUtilities.HandsLog(game));
            return Task.CompletedTask;
        }
    }
}
