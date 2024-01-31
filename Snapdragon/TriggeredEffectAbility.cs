namespace Snapdragon
{
    public record TriggeredEffectAbility<T>(ITrigger Trigger, IEffect Effect)
        : ITriggeredEffectAbility
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
