using Snapdragon.Fluent.Filters;

namespace Snapdragon.Fluent.EventFilters
{
    /// <summary>
    /// A filter that gets cards (or events with cards) on the opposite side as the context card.
    /// </summary>
    public record OtherSideFilter
        : WhereCardFilter<IObjectWithSide>,
            IEventFilter<CardEvent, IObjectWithSide>
    {
        public bool Includes(CardEvent e, IObjectWithSide context, Game game)
        {
            return e.Card.Side != context.Side;
        }

        protected override bool Includes(ICard card, IObjectWithSide context)
        {
            return card.Side != context.Side;
        }
    }
}
