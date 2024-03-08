using Snapdragon.Effects;

namespace Snapdragon.RevealAbilities
{
    public record SwitchSides : IRevealAbility<Card>
    {
        public Game Activate(Game game, Card source)
        {
            var effect = new SwitchCardSide(source);

            return effect.Apply(game);
        }
    }
}
