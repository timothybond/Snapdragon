namespace Snapdragon.Fluent.Selectors
{
    public record EventCardSelector : ISingleItemSelector<ICardInstance, CardEvent, object>
    {
        public ICardInstance? GetOrDefault(CardEvent e, object context, Game game)
        {
            return e.Card;
        }
    }
}
