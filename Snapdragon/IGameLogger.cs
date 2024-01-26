namespace Snapdragon
{
    /// <summary>
    /// Anything that writes a record of what's happening in a Game.
    /// </summary>
    public interface IGameLogger
    {
        void LogGameState(GameState game);

        void LogEvent(Event e);
    }
}
