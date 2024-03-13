namespace Snapdragon.SideFilters
{
    public record BothSides : ISideFilter<object>
    {
        public bool Applies(Side side, object source, Game game)
        {
            return true;
        }
    }
}
