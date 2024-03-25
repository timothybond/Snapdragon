namespace Snapdragon.Fluent.Selectors
{
    public record EventCardSelector : ISingleItemSelector<ICard, CardEvent, object>
    {
        public ICard? GetOrDefault(CardEvent e, object context, Game game)
        {
            return e.Card;
        }
    }
}
