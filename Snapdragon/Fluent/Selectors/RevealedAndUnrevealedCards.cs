namespace Snapdragon.Fluent.Selectors
{
    /// <summary>
    /// A selector that gets all cards at locations, whether or not they are revealed.
    /// </summary>
    public record RevealedAndUnrevealedCards : ISelector<ICardInstance, object>
    {
        public IEnumerable<ICardInstance> Get(object context, Game game)
        {
            return game.AllCardsIncludingUnrevealed;
        }

        public bool Selects(ICardInstance item, object context, Game game)
        {
            return item.State == CardState.InPlay || item.State == CardState.PlayedButNotRevealed;
        }
    }
}
