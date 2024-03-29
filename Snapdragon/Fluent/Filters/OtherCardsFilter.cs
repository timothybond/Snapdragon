﻿namespace Snapdragon.Fluent.Filters
{
    /// <summary>
    /// A filter that selects any card except for the context card,
    /// or any card event that's not regarding the context card.
    /// </summary>
    public record OtherCardsFilter : WhereCardFilter<ICardInstance>, IEventFilter<CardEvent, ICardInstance>
    {
        public bool Includes(CardEvent e, ICardInstance context, Game game)
        {
            return e.Card.Id != context.Id;
        }

        protected override bool Includes(ICardInstance card, ICardInstance context)
        {
            return card.Id != context.Id;
        }
    }
}
