namespace Snapdragon.Fluent.EventFilters
{
    /// <summary>
    /// A filter that gets events that happened on the current turn that the game is on.
    /// </summary>
    public record TurnAfterRevealFilter<TContext> : IEventFilter<Event, TContext>
        where TContext : IRevealableObject
    {
        public bool Includes(Event e, TContext context, Game game)
        {
            // Note TurnRevealed could be null (which will never match)
            return e.Turn - 1 == context.TurnRevealed;
        }
    }
}
