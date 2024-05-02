namespace Snapdragon.Fluent.Selectors
{
    /// <summary>
    /// A selector that gets all cards in play and revealed
    /// (i.e., with state <see cref="CardState.InPlay"/>).
    /// </summary>
    public record RevealedCards : ISelector<ICardInstance, object>
    {
        public IEnumerable<ICardInstance> Get(object context, Game game)
        {
            return game.AllCards;
        }

        public bool Selects(ICardInstance item, object context, Game game)
        {
            return item.State == CardState.InPlay;
        }
    }
}
