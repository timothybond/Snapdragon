namespace Snapdragon
{
    /// <summary>
    /// A single log of something that happened while playing a <see cref="Game"/>.
    ///
    /// Like <see cref="GameRecord"/>, this is primarily for storage.
    /// </summary>
    /// <param name="GameId">Unique identifier of the game.</param>
    /// <param name="Order">Order of this log (must be unique across all stored logs).</param>
    /// <param name="Turn">Turn that produced this log.</param>
    /// <param name="Contents">Arbitrary log contents.</param>
    public record GameLogRecord(Guid GameId, int Order, int Turn, string Contents) { }
}
