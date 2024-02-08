namespace Snapdragon
{
    public interface ITrigger<TEvent>
    {
        bool IsMet(TEvent e, Game game);
    }

    public interface ITrigger<TSource, TEvent>
    {
        bool IsMet(TEvent e, Game game, TSource source);
    }

    /// <summary>
    /// Special instance of <see cref="ITrigger{T}"/> that just wraps a more basic <see cref="ITrigger"/>.
    /// </summary>
    public record WrappedTrigger<TSource, TEvent>(ITrigger<TEvent> Inner)
        : ITrigger<TSource, TEvent>
    {
        public bool IsMet(TEvent e, Game game, TSource source)
        {
            return Inner.IsMet(e, game);
        }
    }
}
