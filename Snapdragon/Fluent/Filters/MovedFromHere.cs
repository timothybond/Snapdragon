using Snapdragon.Events;

namespace Snapdragon.Fluent.Filters
{
    public record MovedFromHere() : IEventFilter<CardMovedEvent, IObjectWithPossibleColumn>
    {
        public bool Includes(CardMovedEvent e, IObjectWithPossibleColumn context, Game game)
        {
            return e.From == context.Column;
        }
    }
}
