namespace Snapdragon
{
    /// <summary>
    /// Anything that writes a record of what's happening in a Game.
    /// </summary>
    public interface IGameLogger
    {
        /// <summary>
        /// Logs the current state of the game.
        /// </summary>
        void LogGameState(Game game);

        /// <summary>
        /// Logs a single event that happened during the game.
        /// </summary>
        /// <param name="e"></param>
        void LogEvent(Event e);

        /// <summary>
        /// Logs the player hands. (Used at the start of each turn.)
        /// </summary>
        /// <param name="game"></param>
        void LogHands(Game game);

        /// <summary>
        /// Logs anything final about the game. Some implementations may use this to batch parts of the logging.
        /// </summary>
        Task LogFinishedGame(Game game);
    }
}
