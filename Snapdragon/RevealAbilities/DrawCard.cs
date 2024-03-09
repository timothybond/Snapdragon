namespace Snapdragon.RevealAbilities
{
    public record DrawCard() : IRevealAbility<CardInstance>
    {
        public Game Activate(Game game, CardInstance source)
        {
            var drawEffect = new Effects.DrawCard(source.Side);
            return drawEffect.Apply(game);
        }
    }
}
