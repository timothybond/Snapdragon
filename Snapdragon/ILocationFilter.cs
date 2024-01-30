namespace Snapdragon
{
    public interface ILocationFilter<T>
    {
        bool Applies(Location location, T source, Game game);
    }
}
