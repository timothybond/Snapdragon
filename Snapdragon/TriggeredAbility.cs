namespace Snapdragon
{
    public record TriggeredAbility<TSource, TEvent>(
        ITrigger<TSource, TEvent> Trigger,
        ISourceTriggeredEffectBuilder<TSource, TEvent> EffectBuilder
    ) : BaseTriggeredAbility<TSource, TEvent>
        where TEvent : Event
    {
        public override bool InHand => false;
        public override bool InDeck => false;
        public override bool DiscardedOrDestroyed => false;

        protected override Game ProcessEvent(Game game, TEvent e, TSource source)
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
