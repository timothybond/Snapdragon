namespace Snapdragon.Fluent.EffectBuilders
{
    public record CreateTriggeredSensorBuilder(ITriggeredAbility<Sensor<ICard>> TriggeredAbility)
        : IEffectBuilder<ICard>
    {
        public IEffect Build(ICard context, Game game)
        {
            return new Snapdragon.Effects.CreateTriggeredSensor(context, TriggeredAbility);
        }
    }
}
