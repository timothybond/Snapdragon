namespace Snapdragon
{
    public interface ILocationFilter<in T>
    {
        bool Applies(Location location, T source, Game game);
    }
}
