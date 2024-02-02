namespace Snapdragon
{
    public interface ISideFilter<T>
    {
        bool Applies(Side side, T source, Game game);
    }
}
