namespace Snapdragon.Fluent.Filters
{
    /// <summary>
    /// A filter for a <see cref="ICardSelector{TContext}"/> that passes each card individually through some check.
    ///
    /// (This is in contrast some some filters, like <see cref="MaxCostOf"/> or <see cref="MinCostOf"/>,
    /// that require the other potential choices for context.)
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public abstract record WhereCardFilter<TContext> : ICardFilter<TContext>
    {
        public IEnumerable<ICard> GetFrom(IEnumerable<ICard> initial, TContext context)
        {
            return initial.Where(c => Includes(c, context));
        }

        protected abstract bool Includes(ICard card, TContext context);
    }
}
