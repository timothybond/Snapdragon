namespace Snapdragon.Fluent.Selectors
{
    /// <summary>
    /// A selector that returns the <see cref="Card"/> that spawned a <see cref="Sensor{Card}"/>,
    /// if it is still in play.
    /// </summary>
    public record SourceCard : ISingleItemSelector<ICardInstance, Sensor<ICard>>
    {
        public ICardInstance? GetOrDefault(Sensor<ICard> context, Game game)
        {
            return game.AllCards.SingleOrDefault(c => c.Id == context.Source.Id);
        }

        public bool Selects(ICardInstance item, Sensor<ICard> context, Game game)
        {
            return item.Id == context.Source.Id;
        }
    }
}
