namespace Snapdragon.LocationFilters
{
    public record ToTheRight() : ILocationFilter<Card>
    {
        public bool Applies(Location location, Card source, GameState game)
        {
            switch (source.Column)
            {
                case Column.Left:
                    return location.Column == Column.Middle;
                case Column.Middle:
                    return location.Column == Column.Right;
                case Column.Right:
                    return false;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
