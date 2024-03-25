namespace Snapdragon.Fluent
{
    public interface ISingleItemFilter<TSelected, in TContext>
        : IFilter<TSelected, TContext>,
            ISingleItemFilter<TSelected, Event, TContext>
    {
        TSelected? GetOrDefault(IEnumerable<TSelected> initial, TContext context, Game game);

        TSelected? ISingleItemFilter<TSelected, Event, TContext>.GetOrDefault(
            IEnumerable<TSelected> initial,
            Event e,
            TContext context,
            Game game
        )
        {
            return this.GetOrDefault(initial, context, game);
        }

        IEnumerable<TSelected> IFilter<TSelected, TContext>.GetFrom(
            IEnumerable<TSelected> initial,
            TContext context,
            Game game
        )
        {
            var item = this.GetOrDefault(initial, context, game);

            if (item != null)
            {
                yield return item;
            }
        }
    }

    public interface ISingleItemFilter<TSelected, in TEvent, in TContext>
        : IFilter<TSelected, TEvent, TContext>
        where TEvent : Event
    {
        TSelected? GetOrDefault(
            IEnumerable<TSelected> initial,
            TEvent e,
            TContext context,
            Game game
        );
    }
}
