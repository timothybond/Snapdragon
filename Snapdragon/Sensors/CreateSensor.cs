namespace Snapdragon.Sensors
{
    public record CreateSensor<TEvent>(SensorBuilder<TEvent> Builder) : IRevealAbility<ICard>
        where TEvent : Event
    {
        public Game Activate(Game game, ICard source)
        {
            var sensor = this.Builder.Build(game, source) with { TurnRevealed = game.Turn };

            return game.WithSensor(sensor);
        }
    }
}
