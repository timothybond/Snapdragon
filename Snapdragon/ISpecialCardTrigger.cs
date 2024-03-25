namespace Snapdragon
{
    public interface ISpecialCardTrigger
    {
        bool WhenDiscardedOrDestroyed { get; }
        bool WhenInHand { get; }
        bool WhenInDeck { get; }
    }
}
