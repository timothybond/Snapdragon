namespace Snapdragon
{
    public interface ITrigger
    {
        bool IsMet(Event e, Game game);
    }

    public interface ITrigger<T>
    {
        bool IsMet(Event e, Game game, T source);
    }

    /// <summary>
    /// Special instance of <see cref="ITrigger{T}"/> that just wraps a more basic <see cref="ITrigger"/>.
    /// </summary>
    public record WrappedTrigger<T>(ITrigger Inner) : ITrigger<T>
    {
        public bool IsMet(Event e, Game game, T source)
        {
            return Inner.IsMet(e, game);
        }
    }
}
