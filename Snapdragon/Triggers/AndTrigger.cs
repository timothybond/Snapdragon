namespace Snapdragon.Triggers
{
    public record AndTrigger<TEvent>(ITrigger<TEvent> First, ITrigger<TEvent> Second) : ITrigger<TEvent>
    {
        public bool IsMet(TEvent e, Game game)
        {
            return First.IsMet(e, game) && Second.IsMet(e, game);
        }
    }
}
