using Snapdragon.Fluent.Filters;

namespace Snapdragon.Fluent.Selectors
{
    /// <summary>
    /// Used as a selector/filter for getting the context card, or events targeting it.
    /// </summary>
    public record Self
        : WhereCardFilter<ICard>,
            ISingleCardSelector<ICard>,
            IEventFilter<CardEvent, ICard>
    {
        public ICard? GetOrDefault(ICard context, Game game)
        {
            return game.AllCards.SingleOrDefault(c => c.Id == context.Id);
        }

        public bool Includes(CardEvent e, ICard context, Game game)
        {
            return e.Card.Id == context.Id;
        }

        protected override bool Includes(ICard card, ICard context)
        {
            return card.Id == context.Id;
        }
    }
}
