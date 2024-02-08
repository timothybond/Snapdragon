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
            throw new NotImplementedException();
        }
    }
}
