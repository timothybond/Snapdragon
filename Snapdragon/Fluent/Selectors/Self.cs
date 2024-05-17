using Snapdragon.Fluent.Filters;

namespace Snapdragon.Fluent.Selectors
{
    /// <summary>
    /// Used as a selector/filter for getting the context card, or events targeting it.
    /// </summary>
    public record Self
        : WhereCardFilter<ICardInstance>,
            ISingleItemSelector<ICardInstance, ICardInstance>,
            IEventFilter<CardEvent, ICardInstance>
    {
        public ICardInstance? GetOrDefault(ICardInstance context, Game game)
        {
            return game.GetCardInstance(context.Id);
        }

        public bool Includes(CardEvent e, ICardInstance context, Game game)
        {
            return e.Card.Id == context.Id;
        }

        public bool Selects(ICardInstance item, ICardInstance context, Game game)
        {
            return item.Id == context.Id;
        }

        public override bool Applies(ICardInstance card, ICardInstance context, Game game)
        {
            return card.Id == context.Id;
        }
    }
}
