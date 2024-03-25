namespace Snapdragon.Fluent
{
    public interface ISingleItemSelector<TSelected, in TContext>
        : ISelector<TSelected, TContext>,
            ISingleItemSelector<TSelected, Event, TContext>
    {
        TSelected? GetOrDefault(TContext context, Game game);

        TSelected? ISingleItemSelector<TSelected, Event, TContext>.GetOrDefault(
            Event e,
            TContext context,
            Game game
        )
        {
            return this.GetOrDefault(context, game);
        }

        IEnumerable<TSelected> ISelector<TSelected, TContext>.Get(TContext context, Game game)
        {
            var item = this.GetOrDefault(context, game);
            if (item != null)
            {
                yield return item;
            }
        }

        IEnumerable<TSelected> ISelector<TSelected, Event, TContext>.Get(
            Event e,
            TContext context,
            Game game
        )
        {
            return this.Get(context, game);
        }
    }

    public interface ISingleItemSelector<TSelected, in TEvent, in TContext>
        : ISelector<TSelected, TEvent, TContext>
        where TEvent : Event
    {
        TSelected? GetOrDefault(TEvent e, TContext context, Game game);

        IEnumerable<TSelected> ISelector<TSelected, TEvent, TContext>.Get(
            TEvent e,
            TContext context,
            Game game
        )
        {
            var item = this.GetOrDefault(e, context, game);
            if (item != null)
            {
                yield return item;
            }
        }
    }
}
