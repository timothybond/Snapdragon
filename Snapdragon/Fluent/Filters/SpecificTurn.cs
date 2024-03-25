namespace Snapdragon.Fluent.EventFilters
{
    /// <summary>
    /// A filter/condition that gets events that happened on the specific turn,
    /// or determines if it is the specific turn.
    /// </summary>
    public record SpecificTurn(int Turn) : IEventFilter<Event, object>, ICondition<object>
    {
        public bool Includes(Event e, object context, Game game)
        {
            return e.Turn == Turn;
        }

        public bool IsMet(object context, Game game)
        {
            return game.Turn == Turn;
        }

        // Note: this invokes interface's default implementation,
        // but it needs to exist because the other interface
        // implements a method of the same name.
        // For some reason I couldn't use the (base) syntax defined at https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-8.0/default-interface-methods.
        // Maybe this is because it's a generic interface, but I haven't figured out for sure.
        public bool IsMet(Event e, object context, Game game)
        {
            return ((IEventFilter<Event, object>)this).IsMet(e, context, game);
        }
    }
}
