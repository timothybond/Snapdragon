namespace Snapdragon.Triggers
{
    public record InHandTrigger(ITrigger Trigger) : ITrigger
    {
        public bool IsMet(Event e, Game game)
        {
            return Trigger.IsMet(e, game);
        }
    }
}
