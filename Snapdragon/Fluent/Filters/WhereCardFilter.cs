namespace Snapdragon.Fluent.Filters
{
    /// <summary>
    /// A filter for a <see cref="ISelector{ICard, TContext}"/> that passes each card individually through some check.
    ///
    /// (This is in contrast some some filters, like <see cref="MaxCostOf"/> or <see cref="MinCostOf"/>,
    /// that require the other potential choices for context.)
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public abstract record WhereCardFilter<TContext> : IFilter<ICardInstance, TContext>
    {
        public abstract bool Applies(ICardInstance item, TContext context, Game game);

        public IEnumerable<ICardInstance> GetFrom(IEnumerable<ICardInstance> initial, TContext context, Game game)
        {
            return initial.Where(c => Applies(c, context, game));
        }
    }
}
