namespace Snapdragon.Fluent.EffectBuilders
{
    /// <summary>
    /// An effect builder that destroys the sensor that is the context object.
    /// </summary>
    public record DestroySensorBuilder : IEffectBuilder<Event, Sensor<ICard>>
    {
        public IEffect Build(Event e, Sensor<ICard> context, Game game)
        {
            return new Snapdragon.Effects.DestroySensor<ICard>(context);
        }
    }
}
