namespace Snapdragon.Fluent.Selectors
{
    public record LocationToTheRight : ISelector<Location, IObjectWithColumn>
    {
        public IEnumerable<Location> Get(IObjectWithColumn context, Game game)
        {
            switch (context.Column)
            {
                case Column.Left:
                    yield return game.Middle;
                    break;
                case Column.Middle:
                    yield return game.Right;
                    break;
                case Column.Right:
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public bool Selects(Location item, IObjectWithColumn context, Game game)
        {
            return (context.Column == Column.Middle && item.Column == Column.Right) ||
                (context.Column == Column.Left && item.Column == Column.Middle);
        }
    }
}
