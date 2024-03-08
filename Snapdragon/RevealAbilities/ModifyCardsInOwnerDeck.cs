using Snapdragon.Effects;

namespace Snapdragon.RevealAbilities
{
    public record ModifyCardsInOwnerDeck(ICardModifier Modifier) : IRevealAbility<Card>
    {
        public Game Activate(Game game, Card source)
        {
            var effect = new ModifyCardsInDeck(source.Side, Modifier);

            return effect.Apply(game);
        }
    }
}
