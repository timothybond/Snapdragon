namespace Snapdragon
{
    public interface ISideFilter<in T>
    {
        bool Applies(Side side, T source, Game game);
    }
}
