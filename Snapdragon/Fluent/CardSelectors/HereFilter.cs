namespace Snapdragon.Fluent.CardSelectors
{
    public record HereFilter<TContext> : ICardFilter<TContext>
        where TContext : IObjectWithColumn
    {
        public bool Includes(ICard card, TContext context)
        {
            return card.Column == context.Column;
        }
    }
}
