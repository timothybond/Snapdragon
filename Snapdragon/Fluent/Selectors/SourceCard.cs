namespace Snapdragon.Fluent.Selectors
{
    /// <summary>
    /// A selector that returns the <see cref="Card"/> that spawned a <see cref="Sensor{Card}"/>,
    /// if it is still in play.
    /// </summary>
    public record SourceCard : ICardSelector<Sensor<Card>>
    {
        public IEnumerable<ICard> Get(Sensor<Card> context, Game game)
        {
            return game.AllCards.Where(c => c.Id == context.Source.Id);
        }
    }
}
