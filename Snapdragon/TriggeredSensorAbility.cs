namespace Snapdragon
{
    public record TriggeredSensorAbility<TEvent>(
        ITrigger<TEvent> Trigger,
        ISourceTriggeredEffectBuilder<Sensor<Card>, TEvent> EffectBuilder
    ) : BaseSensorTriggeredAbility<TEvent>
        where TEvent : Event
    {
        protected override Game ProcessEvent(Game game, TEvent e, Sensor<Card> source)
        {
            if (this.Trigger.IsMet(e, game))
            {
                var effect = this.EffectBuilder.Build(game, e, source);
                return effect.Apply(game);
            }

            return game;
        }
    }
}
