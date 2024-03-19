using Snapdragon.Fluent.Filters;

namespace Snapdragon.Fluent.EventFilters
{
    /// <summary>
    /// A filter that gets cards (or events with cards) on the same side as the context card.
    /// </summary>
    public record ThisSideFilter : WhereCardFilter<ICard>, IEventFilter<ICardEvent, ICard>
    {
        public bool Includes(ICardEvent e, ICard context, Game game)
        {
            return e.Card.Side == context.Side;
        }

        protected override bool Includes(ICard card, ICard context)
        {
            return card.Side == context.Side;
        }
    }
}
