namespace Snapdragon.Sensors
{
    public record CreateSensor<TEvent>(SensorBuilder<TEvent> Builder) : IRevealAbility<Card>
        where TEvent : Event
    {
        public Game Activate(Game game, Card source)
        {
            var temporaryCardEffect = this.Builder.Build(game, source);

            return game.WithTemporaryCardEffect(temporaryCardEffect);
        }
    }
}
