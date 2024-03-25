namespace Snapdragon.Fluent
{
    public interface ISelector<out TSelected, in TContext> : ISelector<TSelected, Event, TContext>
    {
        IEnumerable<TSelected> Get(TContext context, Game game);

        IEnumerable<TSelected> ISelector<TSelected, Event, TContext>.Get(
            Event e,
            TContext context,
            Game game
        )
        {
            return this.Get(context, game);
        }
    }

    public interface ISelector<out TSelected, in TEvent, in TContext>
    {
        IEnumerable<TSelected> Get(TEvent e, TContext context, Game game);
    }
}
