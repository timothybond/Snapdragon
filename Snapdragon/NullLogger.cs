namespace Snapdragon
{
    /// <summary>
    /// An implementation of <see cref="IGameLogger"/> that just doesn't log anything.
    /// </summary>
    public class NullLogger : IGameLogger
    {
        public Task LogEvent(Event e)
        {
            return Task.CompletedTask;
        }

        public Task LogFinishedGame(Game game)
        {
            return Task.CompletedTask;
        }

        public Task LogGameState(Game game)
        {
            return Task.CompletedTask;
        }

        public Task LogHands(Game game)
        {
            return Task.CompletedTask;
        }
    }
}
