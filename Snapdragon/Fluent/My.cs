using Snapdragon.Fluent.Filters;
using Snapdragon.Fluent.Selectors;

namespace Snapdragon.Fluent
{
    public static class My
    {
        public static readonly ISelector<ICard, IObjectWithSide> Cards = new RevealedCardsForSide(
            false
        );

        public static readonly ISelector<ICard, ICard> OtherCards = new FilteredSelector<
            ICard,
            ICard
        >(new RevealedCardsForSide(false), new OtherCardsFilter());

        public static readonly ISelector<ICard, IObjectWithSide> Hand = new HandForSide(false);

        public static readonly ISelector<ICard, IObjectWithSide> Library = new LibraryForSide(
            false
        );

        public static readonly ISelector<ICard, IObjectWithSide> Discards = new DiscardedForSide(
            false
        );

        public static readonly ISelector<ICard, IObjectWithSide> Destroyed = new DestroyedForSide(
            false
        );

        public static readonly ISingleItemSelector<Player, IObjectWithSide> Self = new SameSide();
    }
}
