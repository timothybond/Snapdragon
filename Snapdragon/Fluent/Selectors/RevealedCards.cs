namespace Snapdragon.Fluent.Selectors
{
    public record RevealedCards : ISelector<Card, object>
    {
        public IEnumerable<Card> Get(object context, Game game)
        {
            return game.AllCards;
        }
    }
}
