namespace Snapdragon
{
    public interface ITriggeredCardAbility : ITriggeredAbility<ICard>
    {
        bool InHand { get; }
        bool InDeck { get; }
        bool DiscardedOrDestroyed { get; }
    }
}
