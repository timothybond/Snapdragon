namespace Snapdragon.Fluent
{
    public interface ICardFilter<in TContext>
    {
        IEnumerable<ICard> GetFrom(IEnumerable<ICard> initial, TContext context);
    }
}
