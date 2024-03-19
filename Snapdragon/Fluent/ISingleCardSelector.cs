namespace Snapdragon.Fluent
{
    public interface ISingleCardSelector<in TContext>
        : ISingleCardSelector<Event, TContext>,
            ICardSelector<TContext>
    {
        ICard? GetOrDefault(TContext context, Game game);

        ICard? ISingleCardSelector<Event, TContext>.GetOrDefault(
            Event e,
            TContext context,
            Game game
        )
        {
            return this.GetOrDefault(context, game);
        }

        IEnumerable<ICard> ICardSelector<TContext>.Get(TContext context, Game game)
        {
            var card = this.GetOrDefault(context, game);
            if (card != null)
            {
                yield return card;
            }
        }

        IEnumerable<ICard> ICardSelector<Event, TContext>.Get(Event e, TContext context, Game game)
        {
            return this.Get(context, game);
        }
    }

    public interface ISingleCardSelector<in TEvent, in TContext>
    {
        ICard? GetOrDefault(TEvent e, TContext context, Game game);
    }
}
