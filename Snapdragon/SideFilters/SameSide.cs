namespace Snapdragon.SideFilters
{
    public record SameSide : ISideFilter<Card>
    {
        public bool Applies(Side side, Card source, Game game)
        {
            return side == source.Side;
        }
    }
}
