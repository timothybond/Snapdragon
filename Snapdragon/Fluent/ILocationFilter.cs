namespace Snapdragon.Fluent
{
    public interface ILocationFilter<in TContext>
    {
        IEnumerable<Location> GetFrom(IEnumerable<Location> initial, TContext context);
    }
}
