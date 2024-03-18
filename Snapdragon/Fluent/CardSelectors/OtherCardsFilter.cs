namespace Snapdragon.Fluent.CardSelectors
{
    public record OtherCardsFilter<TContext> : ICardFilter<TContext>
        where TContext : ICard
    {
        public bool Includes(ICard card, TContext context)
        {
            return card.Id != context.Id;
        }
    }
}
