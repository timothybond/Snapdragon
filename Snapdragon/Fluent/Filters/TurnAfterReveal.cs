namespace Snapdragon.Fluent.EventFilters
{
    /// <summary>
    /// A filterthat gets events that happened on the turn after the context object
    /// was revealed (or created, for Sensors), or a condition that is met on that turn.
    /// </summary>
    public record TurnAfterReveal<TContext> : IEventFilter<Event, TContext>, ICondition<TContext>
        where TContext : IRevealableObject
    {
        public bool Includes(Event e, TContext context, Game game)
        {
            // Note TurnRevealed could be null (which will never match)
            return e.Turn - 1 == context.TurnRevealed;
        }

        public bool IsMet(TContext context, Game game)
        {
            return game.Turn - 1 == context.TurnRevealed;
        }

        // Note: this invokes interface's default implementation,
        // but it needs to exist because the other interface
        // implements a method of the same name.
        // For some reason I couldn't use the (base) syntax defined at https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-8.0/default-interface-methods.
        // Maybe this is because it's a generic interface, but I haven't figured out for sure.
        public bool IsMet(Event e, TContext context, Game game)
        {
            return ((IEventFilter<Event, object>)this).IsMet(e, context, game);
        }
    }
}
