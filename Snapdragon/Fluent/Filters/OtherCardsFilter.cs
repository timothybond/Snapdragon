namespace Snapdragon.Fluent.Filters
{
    /// <summary>
    /// A filter that selects any card except for the context card.
    /// </summary>
    public record OtherCardsFilter : WhereCardFilter<ICard>
    {
        protected override bool Includes(ICard card, ICard context)
        {
            return card.Id != context.Id;
        }
    }
}
