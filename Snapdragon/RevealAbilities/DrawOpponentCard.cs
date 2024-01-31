namespace Snapdragon.RevealAbilities
{
    public record DrawOpponentCard() : IRevealAbility<Card>
    {
        public Game Activate(Game game, Card source)
        {
            var drawEffect = new Effects.DrawOpponentCard(source.Side);
            return drawEffect.Apply(game);
        }
    }
}
