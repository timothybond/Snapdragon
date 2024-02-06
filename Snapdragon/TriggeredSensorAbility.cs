namespace Snapdragon
{
    public record TriggeredSensorAbility<T>(ITrigger Trigger, IEffect Effect)
        : ISensorTriggeredAbility
    {
        public Game ProcessEvent(Game game, Event e)
        {
            if (this.Trigger.IsMet(e, game))
            {
                return this.Effect.Apply(game);
            }

            return game;
        }
    }
}
