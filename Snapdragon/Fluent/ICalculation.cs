namespace Snapdragon.Fluent
{
    /// <summary>
    /// Any logic that gets a value based on some circumstances.
    /// </summary>
    public interface ICalculation<in TContext> : ICalculation<Event, TContext>
        where TContext : class
    {
        int GetValue(TContext context, Game game);

        int ICalculation<Event, TContext>.GetValue(Event e, TContext context, Game game)
        {
            return this.GetValue(context, game);
        }
    }

    /// <summary>
    /// Any logic that gets a value based on some event and circumstances.
    /// </summary>
    public interface ICalculation<in TEvent, in TContext>
        where TEvent : Event
    {
        int GetValue(TEvent e, TContext context, Game game);
    }

    public record ConstantValue(int Amount) : ICalculation<object>
    {
        public int GetValue(object context, Game game)
        {
            return Amount;
        }
    }
}
