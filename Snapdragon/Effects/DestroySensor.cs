namespace Snapdragon.Effects
{
    public record DestroySensor<TContext>(Sensor<TContext> sensor) : IEffect
    {
        public Game Apply(Game game)
        {
            return game.WithLocation(game[sensor.Column].WithSensorDeleted(sensor.Id));
        }
    }
}
