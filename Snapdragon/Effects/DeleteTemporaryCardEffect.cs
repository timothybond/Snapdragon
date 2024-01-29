namespace Snapdragon.Effects
{
    public record DeleteTemporaryCardEffect(TemporaryEffect<Card> TemporaryCardEffect) : IEffect
    {
        public GameState Apply(GameState game)
        {
            return game.WithTemporaryCardEffectDeleted(TemporaryCardEffect.Id);
        }
    }
}
