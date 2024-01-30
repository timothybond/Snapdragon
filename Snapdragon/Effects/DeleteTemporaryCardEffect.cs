namespace Snapdragon.Effects
{
    public record DeleteTemporaryCardEffect(TemporaryEffect<Card> TemporaryCardEffect) : IEffect
    {
        public Game Apply(Game game)
        {
            return game.WithTemporaryCardEffectDeleted(TemporaryCardEffect.Id);
        }
    }
}
