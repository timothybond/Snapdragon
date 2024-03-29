using Snapdragon.Fluent.Selectors;

namespace Snapdragon.Fluent
{
    public static class Opposing
    {
        // TODO: Make a more straightforward opponent selector
        public static readonly ISingleItemSelector<Player, IObjectWithSide> Player =
            new OtherPlayerSelector<IObjectWithSide>(new SameSide());

        public static readonly ISelector<ICardInstance, IObjectWithSide> Cards = new RevealedCardsForSide(
            true
        );

        public static readonly ISelector<ICard, IObjectWithSide> CardsIncludingUnrevealed =
            new RevealedAndUnrevealedCards().ForPlayer(Player);

        public static readonly ISelector<ICardInstance, IObjectWithSide> Hand = new HandForSide(true);

        public static readonly ISelector<ICardInstance, IObjectWithSide> Discards = new DiscardedForSide(
            true
        );

        public static readonly ISelector<ICardInstance, IObjectWithSide> Destroyed = new DestroyedForSide(
            true
        );
    }
}
