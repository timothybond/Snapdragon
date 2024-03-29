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
            IEnumerable<ICardInstance> toSearch;

            switch (context.State)
            {
                case CardState.InHand:
                    toSearch = game[context.Side].Hand;
                    break;
                case CardState.InLibrary:
                    toSearch = game[context.Side].Library;
                    break;
                case CardState.InPlay:
                    toSearch = game.AllCards;
                    break;
                case CardState.PlayedButNotRevealed:
                    toSearch = game.AllCardsIncludingUnrevealed;
                    break;
                case CardState.Discarded:
                    toSearch = game[context.Side].Discards;
                    break;
                case CardState.Destroyed:
                    toSearch = game[context.Side].Destroyed;
                    break;
                default:
                    throw new NotImplementedException();
            }
            return toSearch.SingleOrDefault(c => c.Id == context.Id);
        }

        public bool Includes(CardEvent e, ICardInstance context, Game game)
        {
            return e.Card.Id == context.Id;
        }

        protected override bool Includes(ICardInstance card, ICardInstance context)
        {
            return card.Id == context.Id;
        }
    }
}
