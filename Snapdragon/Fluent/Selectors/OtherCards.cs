using Snapdragon.Fluent.Filters;

namespace Snapdragon.Fluent.Selectors
{
    /// <summary>
    /// A filter that selects any card except for the context card,
    /// or any card event that's not regarding the context card.
    /// </summary>
    public record OtherCards
        : WhereCardFilter<ICardInstance>,
            IEventFilter<CardEvent, ICardInstance>,
            ISelector<ICard, ICard>
    {
        public IEnumerable<ICard> Get(ICard context, Game game)
        {
            return game.AllCards.Where(c => c.Id != context.Id);
        }

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
