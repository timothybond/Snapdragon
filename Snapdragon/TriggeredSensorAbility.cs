namespace Snapdragon
{
    public record TriggeredSensorAbility<TEvent>(
        ITrigger<TEvent> Trigger,
        ISourceTriggeredEffectBuilder<Sensor<ICard>, TEvent> EffectBuilder
    ) : BaseSensorTriggeredAbility<TEvent>
        where TEvent : Event
    {
        protected override Game ProcessEvent(Game game, TEvent e, Sensor<ICard> source)
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
