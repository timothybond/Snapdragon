namespace Snapdragon.Fluent.EventFilters
{
    /// <summary>
    /// A filter that gets events that happened on the current turn that the game is on.
    /// </summary>
    public record CurrentTurnFilter : IEventFilter<Event, object>
    {
        public bool Includes(Event e, object context, Game game)
        {
            return e.Turn == game.Turn;
        }
    }
}
