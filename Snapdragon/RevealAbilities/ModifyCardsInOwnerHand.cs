using Snapdragon.Effects;

namespace Snapdragon.RevealAbilities
{
    public record ModifyCardsInOwnerHand(ICardModifier Modifier) : IRevealAbility<Card>
    {
        public Game Activate(Game game, Card source)
        {
            var effect = new ModifyCardsInHand(source.Side, Modifier);

            return effect.Apply(game);
        }
    }
}
