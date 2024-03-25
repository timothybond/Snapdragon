using Snapdragon.Events;
using Snapdragon.Fluent.Filters;
using Snapdragon.Fluent.Selectors;

namespace Snapdragon.Fluent
{
    public static class At
    {
        public static ISelector<Location, IObjectWithPossibleColumn> Here => new HereFilter();

        public static ISelector<Location, CardMovedEvent, object> PriorLocation = new MovedFromSelector();
    }
}
