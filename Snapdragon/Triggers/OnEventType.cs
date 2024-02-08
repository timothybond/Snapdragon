namespace Snapdragon.Triggers
{
    public record OnEventType<TEvent>() : ITrigger<TEvent>
    {
        public bool IsMet(TEvent e, Game game)
        {
            return true;
        }
    }
}
