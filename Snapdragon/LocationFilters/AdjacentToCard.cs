namespace Snapdragon.LocationFilters
{
    public record AdjacentToCard() : ILocationFilter<Card>
    {
        public bool Applies(Location location, Card source, GameState game)
        {
            switch (source.Column)
            {
                case Column.Left:
                case Column.Right:
                    return location.Column == Column.Middle;
                case Column.Middle:
                    return location.Column != Column.Middle;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
