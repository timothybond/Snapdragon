namespace Snapdragon.TemporaryEffects
{
    public record CreateTemporaryEffect(TemporaryEffectBuilder Builder) : IRevealAbility<Card>
    {
        public GameState Activate(GameState game, Card source)
        {
            var temporaryCardEffect = this.Builder.Build(game, source);

            return game.WithTemporaryCardEffect(temporaryCardEffect);
        }
    }
}
