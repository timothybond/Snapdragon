namespace Snapdragon.Fluent
{
    public interface ISingleCardFilter<in TContext> : ICardFilter<TContext>
    {
        ICard? GetOrDefault(IEnumerable<ICard> initial, TContext context);

        IEnumerable<ICard> ICardFilter<TContext>.GetFrom(
            IEnumerable<ICard> initial,
            TContext context
        )
        {
            var card = this.GetOrDefault(initial, context);

            if (card != null)
            {
                yield return card;
            }
        }
    }
}
