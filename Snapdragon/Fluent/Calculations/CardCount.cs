namespace Snapdragon.Fluent.Calculations
{
    public record CardCount<TContext>(ICardSelector<TContext> Selector) : ICalculation<TContext>
        where TContext : class
    {
        public int GetValue(TContext context, Game game)
        {
            return Selector.Get(context, game).Count();
        }
    }
}
