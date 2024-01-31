namespace Snapdragon.Sensors
{
    public record CreateSensor(SensorBuilder Builder) : IRevealAbility<Card>
    {
        public Game Activate(Game game, Card source)
        {
            var temporaryCardEffect = this.Builder.Build(game, source);

            return game.WithTemporaryCardEffect(temporaryCardEffect);
        }
    }
}
