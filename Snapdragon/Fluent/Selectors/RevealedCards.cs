namespace Snapdragon.Fluent.Selectors
{
    public record RevealedCards : ICardSelector<object>
    {
        public IEnumerable<ICard> Get(object context, Game game)
        {
            return game.AllCards;
        }
    }
}
