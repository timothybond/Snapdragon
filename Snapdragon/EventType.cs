namespace Snapdragon
{
    public enum EventType
    {
        CardDrawn = 0,
        CardPlayed = 1,
        CardRevealed = 2,
        CardMoved = 3,
        CardDiscarded = 4,
        CardDestroyedFromPlay = 5,
        CardDestroyedFromHand = 6,
        CardDestroyedFromLibrary = 7,
        TurnStarted = 8,
        LocationRevealed = 9,
        TurnEnded = 10,
        GameEnded = 11,
        CardMerged = 12,
        CardAddedToLocation = 13,
        CardAddedToHand = 14,
        CardReturnedToHand = 15,
        CardReturnedToPlay = 16,
        CardSwitchedSides = 17,
    }
}
