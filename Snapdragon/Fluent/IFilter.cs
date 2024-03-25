namespace Snapdragon.Fluent
{
    public interface IFilter<TSelected, in TContext> : IFilter<TSelected, Event, TContext>
    {
        IEnumerable<TSelected> GetFrom(IEnumerable<TSelected> initial, TContext context, Game game);

        IEnumerable<TSelected> IFilter<TSelected, Event, TContext>.GetFrom(
            IEnumerable<TSelected> initial,
            Event e,
            TContext context,
            Game game
        )
        {
            return this.GetFrom(initial, context, game);
        }
    }

    public interface IFilter<TSelected, in TEvent, in TContext>
        where TEvent : Event
    {
        IEnumerable<TSelected> GetFrom(
            IEnumerable<TSelected> initial,
            TEvent e,
            TContext context,
            Game game
        );
    }
}
