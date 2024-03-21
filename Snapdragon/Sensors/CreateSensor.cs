namespace Snapdragon.Sensors
{
    public record CreateSensor<TEvent>(SensorBuilder<TEvent> Builder) : IRevealAbility<Card>
        where TEvent : Event
    {
        public Game Activate(Game game, Card source)
        {
            var sensor = this.Builder.Build(game, source) with { TurnRevealed = game.Turn };

            return game.WithSensor(sensor);
        }
    }
}
