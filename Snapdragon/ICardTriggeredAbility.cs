namespace Snapdragon
{
    public interface ICardTriggeredAbility : ICardAbility
    {
        public GameState ProcessEvent(Event e, Card source);
    }
}
