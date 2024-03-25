namespace Snapdragon.Fluent.Filters
{
    /// <summary>
    /// A filter for a <see cref="ISelector{ICard, TContext}"/> that passes each card individually through some check.
    ///
    /// (This is in contrast some some filters, like <see cref="MaxCostOf"/> or <see cref="MinCostOf"/>,
    /// that require the other potential choices for context.)
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public abstract record WhereCardFilter<TContext> : IFilter<ICard, TContext>
    {
        public IEnumerable<ICard> GetFrom(IEnumerable<ICard> initial, TContext context, Game game)
        {
            return initial.Where(c => Includes(c, context));
        }

        protected abstract bool Includes(ICard card, TContext context);
    }
}
