﻿namespace Snapdragon.Fluent.Selectors
{
    public record AdjacentLocations : ISelector<Location, IObjectWithColumn>
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

        public bool Selects(Location item, IObjectWithColumn context, Game game)
        {
            if (item.Column == context.Column)
            {
                return false;
            }

            return item.Column == Column.Middle || context.Column == Column.Middle;
        }
    }
}
