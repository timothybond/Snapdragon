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
            IEventFilter<CardEvent, IObjectWithPossibleColumn>,
            ISingleItemSelector<Location, IObjectWithPossibleColumn>
    {
        protected override bool Includes(ICard card, IObjectWithPossibleColumn context)
        {
            return card.Column == context.Column;
        }

        public bool Includes(CardEvent e, IObjectWithPossibleColumn context, Game game)
        {
            return e.Card.Column == context.Column;
        }

        public Location? GetOrDefault(IObjectWithPossibleColumn context, Game game)
        {
            if (context.Column == null)
            {
                return null;
            }

            return game[context.Column.Value];
        }
    }
}
