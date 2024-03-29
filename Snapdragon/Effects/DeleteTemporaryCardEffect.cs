namespace Snapdragon.Effects
{
    public record DeleteTemporaryCardEffect(Sensor<ICard> TemporaryCardEffect) : IEffect
    {
        public Game Apply(Game game)
        {
            return game.WithSensorDeleted(TemporaryCardEffect.Id);
        }
    }
}
