namespace Snapdragon.Triggers
{
    public record ConditionTrigger(Func<GameState, bool> Condition) : ITrigger
    {
        public bool IsMet(Event e, GameState game)
        {
            return this.Condition(game);
        }
    }
}
