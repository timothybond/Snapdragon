using Snapdragon.Fluent.Selectors;

namespace Snapdragon.Fluent
{
    public static class My
    {
        public static readonly ICardSelector<ICard> Cards = new RevealedCardsForSide(false);

        public static readonly ICardSelector<ICard> Hand = new HandForSide(false);

        public static readonly ICardSelector<ICard> Discards = new DiscardedForSide(false);

        public static readonly ICardSelector<ICard> Destroyed = new DestroyedForSide(false);
    }
}
