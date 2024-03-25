namespace Snapdragon.Fluent.Filters
{
    /// <summary>
    /// A filter that selects any card except for the context card,
    /// or any card event that's not regarding the context card.
    /// </summary>
    public record OtherCardsFilter : WhereCardFilter<ICard>, IEventFilter<CardEvent, ICard>
    {
        public bool Includes(CardEvent e, ICard context, Game game)
        {
            return e.Card.Id != context.Id;
        }

        protected override bool Includes(ICard card, ICard context)
        {
            return card.Id != context.Id;
        }
    }
}
