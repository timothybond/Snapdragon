namespace Snapdragon.Fluent.Selectors
{
    /// <summary>
    /// A selector that gets all cards at locations, whether or not they are revealed.
    /// </summary>
    public record RevealedAndUnrevealedCards : ISelector<Card, object>
    {
        public IEnumerable<Card> Get(object context, Game game)
        {
            return game.AllCardsIncludingUnrevealed;
        }
    }
}
