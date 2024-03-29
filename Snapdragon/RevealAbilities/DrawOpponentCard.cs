namespace Snapdragon.RevealAbilities
{
    public record DrawOpponentCard() : IRevealAbility<ICard>
    {
        public Game Activate(Game game, ICard source)
        {
            var drawEffect = new Effects.DrawOpponentCard(source.Side);
            return drawEffect.Apply(game);
        }
    }
}
