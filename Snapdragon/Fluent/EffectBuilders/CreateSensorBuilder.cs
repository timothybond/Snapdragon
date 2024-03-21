namespace Snapdragon.Fluent.EffectBuilders
{
    public record CreateSensorBuilder(ITriggeredAbility<Sensor<Card>> TriggeredAbility)
        : IEffectBuilder<Card>
    {
        public IEffect Build(Card context, Game game)
        {
            return new Snapdragon.Effects.CreateTriggeredSensor(context, TriggeredAbility);
        }
    }
}
