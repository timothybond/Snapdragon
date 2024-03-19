namespace Snapdragon.Fluent
{
    public interface ICardSelector<in TContext> : ICardSelector<Event, TContext>
    {
        IEnumerable<ICard> Get(TContext context, Game game);

        IEnumerable<ICard> ICardSelector<Event, TContext>.Get(Event e, TContext context, Game game)
        {
            return this.Get(context, game);
        }
    }

    public interface ICardSelector<in TEvent, in TContext>
    {
        IEnumerable<ICard> Get(TEvent e, TContext context, Game game);
    }
}
