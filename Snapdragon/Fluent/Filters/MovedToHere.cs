using Snapdragon.Events;

namespace Snapdragon.Fluent.Filters
{
    public record MovedToHere() : IEventFilter<CardMovedEvent, IObjectWithColumn>
    {
        public bool Includes(CardMovedEvent e, IObjectWithColumn context, Game game)
        {
            return e.To == context.Column;
        }
    }
}
