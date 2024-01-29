namespace Snapdragon.Triggers
{
    public record AndTrigger(ITrigger First, ITrigger Second) : ITrigger
    {
        public bool IsMet(Event e, GameState game)
        {
            return First.IsMet(e, game) && Second.IsMet(e, game);
        }
    }
}
