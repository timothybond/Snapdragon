namespace Snapdragon
{
    public record TriggeredSensorAbility(
        ITrigger Trigger,
        ISourceTriggeredEffectBuilder<Sensor<Card>> EffectBuilder
    ) : ISensorTriggeredAbility
    {
        public Game ProcessEvent(Game game, Event e, Sensor<Card> source)
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
