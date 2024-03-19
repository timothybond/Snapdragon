using Snapdragon.Events;
using Snapdragon.Fluent.Filters;
using Snapdragon.Fluent.Selectors;

namespace Snapdragon.Fluent
{
    public static class At
    {
        public static ILocationSelector<IObjectWithPossibleColumn> Here => new HereFilter();

        public static ILocationSelector<CardMovedEvent, object> PriorLocation = new MovedFromSelector();
    }
}
