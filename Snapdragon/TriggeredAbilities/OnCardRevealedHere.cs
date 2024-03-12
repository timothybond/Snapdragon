namespace Snapdragon.TriggeredAbilities
{
    public record OnCardRevealedHere : ITriggeredAbility<Location>
    {
        public Game ProcessEvent(Game game, Event e, Location source)
        {
            throw new NotImplementedException();
        }
    }
}
