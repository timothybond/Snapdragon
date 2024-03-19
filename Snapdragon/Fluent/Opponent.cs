using Snapdragon.Fluent.Selectors;

namespace Snapdragon.Fluent
{
    public static class Opposing
    {
        public static readonly ICardSelector<ICard> Cards = new RevealedCardsForSide(true);

        public static readonly ICardSelector<ICard> Hand = new HandForSide(true);

        public static readonly ICardSelector<ICard> Discards = new DiscardedForSide(true);

        public static readonly ICardSelector<ICard> Destroyed = new DestroyedForSide(true);
    }
}
