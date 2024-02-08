using Snapdragon.Events;

namespace Snapdragon.Triggers
{
    public record OnTurnEnded() : OnEventType<TurnEndedEvent>() { }
}
