namespace Snapdragon.Fluent
{
    public interface ISelector<TSelected, in TContext> : ISelector<TSelected, Event, TContext>
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

        /// <summary>
        /// Checks whether the selector will select a particular item.
        ///
        /// The main purpose of this is to be more efficient in cases where
        /// we want to check selection, so we don't have to just call
        /// <see cref="Get(TContext, Game)"/> and check if an item is contained in the result.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="context"></param>
        /// <param name="game"></param>
        /// <returns></returns>
        bool Selects(TSelected item, TContext context, Game game);
    }

    public interface ISelector<out TSelected, in TEvent, in TContext>
    {
        IEnumerable<TSelected> Get(TEvent e, TContext context, Game game);
    }
}
