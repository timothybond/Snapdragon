namespace Snapdragon.Triggers
{
    public static class TriggerExtensions
    {
        public static ITrigger And(this ITrigger first, ITrigger second)
        {
            return new AndTrigger(first, second);
        }
    }
}
