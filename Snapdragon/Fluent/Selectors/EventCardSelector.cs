namespace Snapdragon.Fluent.Selectors
{
    public record EventCardSelector : ICardSelector<CardEvent, object>
    {
        public IEnumerable<ICard> Get(CardEvent e, object context, Game game)
        {
            yield return e.Card;
        }
    }
}
