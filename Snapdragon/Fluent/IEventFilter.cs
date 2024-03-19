namespace Snapdragon.Fluent
{
    public interface IEventFilter<in TEvent, in TContext>
    {
        bool Includes(TEvent e, TContext context, Game game);
    }
}
