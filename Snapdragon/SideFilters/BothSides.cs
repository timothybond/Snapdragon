namespace Snapdragon.SideFilters
{
    public record BothSides<T> : ISideFilter<T>
    {
        public bool Applies(Side side, T source, Game game)
        {
            return true;
        }
    }
}
