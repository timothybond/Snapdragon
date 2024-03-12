namespace Snapdragon
{
    public record TriggeredCardAbility<TEvent>(
        ITrigger<ICard, TEvent> Trigger,
        ISourceTriggeredEffectBuilder<ICard, TEvent> EffectBuilder
    ) : BaseTriggeredCardAbility<TEvent>
        where TEvent : Event
    {
        public override bool InHand => false;
        public override bool InDeck => false;
        public override bool DiscardedOrDestroyed => false;

        protected override Game ProcessEvent(Game game, TEvent e, ICard source)
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
