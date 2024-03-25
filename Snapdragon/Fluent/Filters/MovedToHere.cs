using Snapdragon.Events;

namespace Snapdragon.Fluent.Filters
{
    public record MovedToHere() : IEventFilter<CardMovedEvent, IObjectWithPossibleColumn>
    {
        public bool Includes(CardMovedEvent e, IObjectWithPossibleColumn context, Game game)
        {
            return e.To == context.Column;
        }
    }
}
