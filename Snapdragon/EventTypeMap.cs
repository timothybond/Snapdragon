using Snapdragon.Events;

namespace Snapdragon
{
    public static class EventTypeMap
    {
        public static EventType Get<TEvent>()
            where TEvent : Event
        {
            if (typeof(TEvent) == typeof(CardAddedToHandEvent))
            {
                return EventType.CardAddedToHand;
            }
            else if (typeof(TEvent) == typeof(CardAddedToLocationEvent))
            {
                return EventType.CardAddedToLocation;
            }
            else if (typeof(TEvent) == typeof(CardDestroyedFromPlayEvent))
            {
                return EventType.CardDestroyedFromPlay;
            }
            else if (typeof(TEvent) == typeof(CardDiscardedEvent))
            {
                return EventType.CardDiscarded;
            }
            else if (typeof(TEvent) == typeof(CardDrawnEvent))
            {
                return EventType.CardDrawn;
            }
            else if (typeof(TEvent) == typeof(CardMergedEvent))
            {
                return EventType.CardMerged;
            }
            else if (typeof(TEvent) == typeof(CardMovedEvent))
            {
                return EventType.CardMoved;
            }
            else if (typeof(TEvent) == typeof(CardPlayedEvent))
            {
                return EventType.CardPlayed;
            }
            else if (typeof(TEvent) == typeof(CardReturnedToHand))
            {
                return EventType.CardReturnedToHand;
            }
            else if (typeof(TEvent) == typeof(CardReturnedToPlay))
            {
                return EventType.CardReturnedToPlay;
            }
            else if (typeof(TEvent) == typeof(CardRevealedEvent))
            {
                return EventType.CardRevealed;
            }
            else if (typeof(TEvent) == typeof(CardSwitchedSidesEvent))
            {
                return EventType.CardSwitchedSides;
            }
            else if (typeof(TEvent) == typeof(GameEndedEvent))
            {
                return EventType.GameEnded;
            }
            else if (typeof(TEvent) == typeof(LocationRevealedEvent))
            {
                return EventType.LocationRevealed;
            }
            else if (typeof(TEvent) == typeof(TurnEndedEvent))
            {
                return EventType.TurnEnded;
            }
            else if (typeof(TEvent) == typeof(TurnStartedEvent))
            {
                return EventType.TurnStarted;
            }
            else
            {
                throw new NotImplementedException(
                    $"Enum value mapping for event type '{typeof(TEvent).Name}' not found."
                );
            }
        }
    }
}
