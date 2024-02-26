namespace Snapdragon
{
    /// <summary>
    /// A simplified record of a game, used for recording rather than actual play.
    /// </summary>
    /// <param name="GameId">Unique identifier of the game.</param>
    /// <param name="TopPlayerId">Unique identifier of the top player.</param>
    /// <param name="BottomPlayerId">Unique identifier of the bottom player.</param>
    /// <param name="Winner">Side of the winning player (null for a tie).</param>
    /// <param name="ExperimentId">Unique identifier of the linked experiment, if any.</param>
    /// <param name="Generation">Generation count for an experiment, if any.</param>
    public record GameRecord(
        Guid GameId,
        Guid TopPlayerId,
        Guid BottomPlayerId,
        Side? Winner = null,
        Guid? ExperimentId = null,
        int? Generation = null
    ) { }
}
