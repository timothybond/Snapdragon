namespace Snapdragon
{
    /// <summary>
    /// An implementation of <see cref="IGameLogger"/> that just doesn't log anything.
    /// </summary>
    public class NullLogger : IGameLogger
    {
        public void LogEvent(Event e) { }

        public void LogGameState(Game game) { }

        public void LogHands(Game game) { }
    }
}
