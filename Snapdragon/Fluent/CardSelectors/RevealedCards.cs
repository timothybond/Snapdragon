namespace Snapdragon.Fluent.CardSelectors
{
    public record RevealedCards : ICardSelector<object>
    {
        public IEnumerable<ICard> Get(object context, Game game)
        {
            return game.AllCards;
        }
    }
}
