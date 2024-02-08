namespace Snapdragon.Triggers
{
    public static class TriggerExtensions
    {
        public static ITrigger<TEvent> And<TEvent>(
            this ITrigger<TEvent> first,
            ITrigger<TEvent> second
        )
        {
            return new AndTrigger<TEvent>(first, second);
        }
    }
}
