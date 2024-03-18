namespace Snapdragon.Fluent
{
    public interface ICardSelector<in TContext>
    {
        IEnumerable<ICard> Get(TContext context, Game game);
    }
}
