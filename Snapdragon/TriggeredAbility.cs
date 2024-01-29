namespace Snapdragon
{
    public record TriggeredAbility<T>(ITrigger Trigger, IEffect Effect) : ITriggeredAbility<T>
    {
        public GameState ProcessEvent(GameState game, Event e)
        {
            if (this.Trigger.IsMet(e, game))
            {
                return this.Effect.Apply(game);
            }

            return game;
        }
    }
}
