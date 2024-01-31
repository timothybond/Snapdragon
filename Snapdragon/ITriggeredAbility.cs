namespace Snapdragon
{
    public interface ITriggeredAbility<T> : IAbility<T>
    {
        bool InHand { get; }
        bool InDeck { get; }

        Game ProcessEvent(Game game, Event e, T source);
    }
}
