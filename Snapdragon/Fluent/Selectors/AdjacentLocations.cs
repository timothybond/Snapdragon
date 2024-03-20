namespace Snapdragon.Fluent.Selectors
{
    public record AdjacentLocations : ILocationSelector<IObjectWithColumn>
    {
        public IEnumerable<Location> Get(IObjectWithColumn context, Game game)
        {
            switch (context.Column)
            {
                case Column.Left:
                case Column.Right:
                    yield return game.Middle;
                    break;
                case Column.Middle:
                    yield return game.Left;
                    yield return game.Right;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
