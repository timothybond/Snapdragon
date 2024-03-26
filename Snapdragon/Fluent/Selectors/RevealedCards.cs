namespace Snapdragon.Fluent.Selectors
{
    /// <summary>
    /// A selector that gets all cards in play and revealed
    /// (i.e., with state <see cref="CardState.InPlay"/>).
    /// </summary>
    public record RevealedCards : ISelector<Card, object>
    {
        public IEnumerable<Card> Get(object context, Game game)
        {
            return game.AllCards;
        }
    }
}
