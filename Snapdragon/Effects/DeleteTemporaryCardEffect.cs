namespace Snapdragon.Effects
{
    public record DeleteTemporaryCardEffect(Sensor<Card> TemporaryCardEffect) : IEffect
    {
        public Game Apply(Game game)
        {
            return game.WithTemporaryCardEffectDeleted(TemporaryCardEffect.Id);
        }
    }
}
