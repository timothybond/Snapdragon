using Snapdragon.Fluent.Filters;
using Snapdragon.Fluent.Selectors;

namespace Snapdragon.Fluent
{
    public static class My
    {
        public static readonly ISelector<ICardInstance, IObjectWithSide> Cards = new RevealedCardsForSide(
            false
        );

        public static readonly ISelector<
            ICard,
            IObjectWithSide
        > CardsIncludingUnrevealed = new RevealedAndUnrevealedCards().ForPlayer(Self);

        public static readonly ISelector<ICardInstance, ICardInstance> OtherCards = new FilteredSelector<
            ICardInstance,
            ICardInstance
        >(new RevealedCardsForSide(false), new OtherCardsFilter());

        public static readonly ISelector<ICardInstance, IObjectWithSide> Hand = new HandForSide(false);

        public static readonly ISelector<ICardInstance, IObjectWithSide> Library = new LibraryForSide(
            false
        );

        public static readonly ISelector<ICardInstance, IObjectWithSide> Discards = new DiscardedForSide(
            false
        );

        public static readonly ISelector<ICardInstance, IObjectWithSide> Destroyed = new DestroyedForSide(
            false
        );

        public static readonly ISingleItemSelector<Player, IObjectWithSide> Self = new SameSide();
    }
}
