namespace Snapdragon.Triggers
{
    public record ConditionTrigger(Func<Game, bool> Condition) : ITrigger
    {
        public bool IsMet(Event e, Game game)
        {
            return this.Condition(game);
        }
    }
}
