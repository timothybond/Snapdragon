
namespace Snapdragon.Fluent.CardSelectors
{
    public record Self : ICardSelector<Card>
    {
        public IEnumerable<ICard> Get(Card context, Game game)
        {
            return game[context.Column][context.Side].Where(c => c.Id == context.Id);
        }
    }
}
