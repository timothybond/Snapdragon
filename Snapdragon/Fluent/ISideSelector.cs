namespace Snapdragon.Fluent
{
    public interface ISideSelector<in TContext>
    {
        IEnumerable<Side> Get(TContext context, Game game);
    }
}
