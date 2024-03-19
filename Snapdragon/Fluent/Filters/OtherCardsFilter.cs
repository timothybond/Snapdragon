namespace Snapdragon.Fluent.Filters
{
    /// <summary>
    /// A filter that selects any card except for the context card.
    /// </summary>
    public record OtherCardsFilter<TContext> : WhereCardFilter<TContext>
        where TContext : ICard
    {
        protected override bool Includes(ICard card, TContext context)
        {
            return card.Id != context.Id;
        }
    }
}
