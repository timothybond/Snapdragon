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
            ISelector<ICardInstance, ICard>
    {
        public IEnumerable<ICardInstance> Get(ICard context, Game game)
        {
            return game.AllCards.Where(c => c.Id != context.Id);
        }

        public bool Includes(CardEvent e, ICardInstance context, Game game)
        {
            return e.Card.Id != context.Id;
        }

        public bool Selects(ICardInstance item, ICard context, Game game)
        {
            return item.Id != context.Id && item.State == CardState.InPlay;
        }

        public override bool Applies(ICardInstance card, ICardInstance context, Game game)
        {
            return card.Id != context.Id;
        }
    }
}
