namespace Snapdragon.Fluent.Selectors
{
    public record LocationToTheRight : ILocationSelector<IObjectWithColumn>
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
    }
}
