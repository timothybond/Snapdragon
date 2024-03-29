namespace Snapdragon
{
    public record TriggeredCardAbility<TEvent>(
        ITrigger<ICardInstance, TEvent> Trigger,
        ISourceTriggeredEffectBuilder<ICardInstance, TEvent> EffectBuilder
    ) : BaseTriggeredCardAbility<TEvent>
        where TEvent : Event
    {
        public override bool WhenInHand => false;
        public override bool WhenInDeck => false;
        public override bool WhenDiscardedOrDestroyed => false;

        protected override Game ProcessEvent(Game game, TEvent e, ICardInstance source)
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
