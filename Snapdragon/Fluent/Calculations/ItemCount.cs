namespace Snapdragon.Fluent.Calculations
{
    public record ItemCount<TSelected, TContext>(ISelector<TSelected, TContext> Selector)
        : ICalculation<TContext>
        where TContext : class
    {
        public int GetValue(TContext context, Game game)
        {
            return Selector.Get(context, game).Count();
        }
    }
}
