namespace Snapdragon.Fluent.Filters
{
    /// <summary>
    /// A filter for "where the context is".
    ///
    /// Can be used to filter cards (i.e., cards in that location),
    /// events (i.e., card events taking place in that location),
    /// or locations themselves.
    /// /// </summary>
    public record HereFilter
        : WhereCardFilter<IObjectWithPossibleColumn>,
            IEventFilter<ICardEvent, IObjectWithPossibleColumn>,
            ILocationSelector<IObjectWithPossibleColumn>
    {
        protected override bool Includes(ICard card, IObjectWithPossibleColumn context)
        {
            return card.Column == context.Column;
        }

        public bool Includes(ICardEvent e, IObjectWithPossibleColumn context, Game game)
        {
            return e.Card.Column == context.Column;
        }

        public IEnumerable<Location> Get(IObjectWithPossibleColumn context, Game game)
        {
            if (context.Column != null)
            {
                yield return game[context.Column.Value];
            }
        }
    }
}
