namespace Snapdragon.LocationFilters
{
    public record ToTheRight() : ILocationFilter<ICard>
    {
        public bool Applies(Location location, ICard source, Game game)
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
