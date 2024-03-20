namespace Snapdragon.Fluent.Selectors
{
    public record EventCardSelector : ICardSelector<ICardEvent, object>
    {
        public IEnumerable<ICard> Get(ICardEvent e, object context, Game game)
        {
            yield return e.Card;
        }
    }
}
