namespace Snapdragon.RevealAbilities
{
    public record DrawCard() : IRevealAbility<Card>
    {
        public Game Activate(Game game, Card source)
        {
            var drawEffect = new Effects.DrawCard(source.Side);
            return drawEffect.Apply(game);
        }
    }
}
