namespace Snapdragon.Fluent.Selectors
{
    public class All
    {
        public static readonly RevealedCards Cards = new RevealedCards();

        public static readonly OtherCards OtherCards = new OtherCards();

        public static readonly RevealedAndUnrevealedCards CardsIncludingUnrevealed =
            new RevealedAndUnrevealedCards();

        public static readonly BothPlayers Players = new BothPlayers();
    }
}
