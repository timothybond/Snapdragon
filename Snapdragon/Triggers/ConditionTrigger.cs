namespace Snapdragon.Triggers
{
    /// <summary>
    /// A trigger that isn't based on an event, but just checks something about the game state.
    ///
    /// Because triggers always only get checked in response to events, this is best used
    /// as a modified for another trigger, combined in an <see cref="AndTrigger"/>.
    /// </summary>
    /// <param name="Condition"></param>
    public record ConditionTrigger(Func<Game, bool> Condition) : ITrigger
    {
        public bool IsMet(Event e, Game game)
        {
            return this.Condition(game);
        }
    }
}
