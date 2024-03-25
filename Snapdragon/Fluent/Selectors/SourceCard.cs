namespace Snapdragon.Fluent.Selectors
{
    /// <summary>
    /// A selector that returns the <see cref="Card"/> that spawned a <see cref="Sensor{Card}"/>,
    /// if it is still in play.
    /// </summary>
    public record SourceCard : ISingleItemSelector<ICard, Sensor<Card>>
    {
        public ICard? GetOrDefault(Sensor<Card> context, Game game)
        {
            return game.AllCards.SingleOrDefault(c => c.Id == context.Source.Id);
        }
    }
}
