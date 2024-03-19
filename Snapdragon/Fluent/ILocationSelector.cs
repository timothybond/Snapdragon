namespace Snapdragon.Fluent
{
    public interface ILocationSelector<in TContext> : ILocationSelector<Event, TContext>
    {
        IEnumerable<Location> Get(TContext context, Game game);

        IEnumerable<Location> ILocationSelector<Event, TContext>.Get(
            Event e,
            TContext context,
            Game game
        )
        {
            return this.Get(context, game);
        }
    }

    public interface ILocationSelector<in TEvent, in TContext>
    {
        IEnumerable<Location> Get(TEvent e, TContext context, Game game);
    }
}
