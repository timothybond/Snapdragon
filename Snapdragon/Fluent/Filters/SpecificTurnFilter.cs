namespace Snapdragon.Fluent.EventFilters
{
    /// <summary>
    /// A filter that gets events that happened on the current turn.
    /// </summary>
    public record SpecificTurnFilter(int Turn) : IEventFilter<Event, object>
    {
        public bool Includes(Event e, object context, Game game)
        {
            return e.Turn == Turn;
        }
    }
}
