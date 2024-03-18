namespace Snapdragon.Fluent.Conditions
{
    public record LocationFullCondition<TContext> : ICondition<TContext>
        where TContext : ICard, IObjectWithColumn
    {
        public bool IsMet(TContext context, Game game)
        {
            // TODO: Handle effects that limit slots
            return game[context.Column][context.Side].Count == Max.CardsPerLocation;
        }
    }
}
