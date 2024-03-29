using Snapdragon.Effects;

namespace Snapdragon.RevealAbilities
{
    public record SwitchSides : IRevealAbility<ICard>
    {
        public Game Activate(Game game, ICard source)
        {
            var effect = new SwitchCardSide(source);

            return effect.Apply(game);
        }
    }
}
