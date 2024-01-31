namespace Snapdragon
{
    public record TriggeredAbility<T>(
        ITrigger<T> Trigger,
        ISourceTriggeredEffectBuilder<T> EffectBuilder
    ) : ITriggeredAbility<T>
    {
        public bool InHand => false;

        public bool InDeck => false;

        public Game ProcessEvent(Game game, Event e, T source)
        {
            if (this.Trigger.IsMet(e, game, source))
            {
                var effect = EffectBuilder.Build(game, e, source);
                return effect.Apply(game);
            }

            return game;
        }
    }
}
