namespace Snapdragon.Fluent.EffectBuilders
{
    /// <summary>
    /// An effect builder that destroys the sensor that is the context object.
    /// </summary>
    public record DestroySensorBuilder : IEffectBuilder<Event, Sensor<Card>>
    {
        public IEffect Build(Event e, Sensor<Card> context, Game game)
        {
            return new Snapdragon.Effects.DestroySensor<Card>(context);
        }
    }
}
