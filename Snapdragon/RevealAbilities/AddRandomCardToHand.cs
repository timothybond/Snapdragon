using Snapdragon.CardDefinitionFilters;

namespace Snapdragon.RevealAbilities
{
    public record AddRandomCardToHand(ICardDefinitionFilter Filter) : IRevealAbility<Card>
    {
        public AddRandomCardToHand()
            : this(new AnyCardDefinition()) { }

        public Game Activate(Game game, Card source)
        {
            var addRandomCardEffect = new Effects.AddRandomCardToHand(source.Side, Filter);
            return addRandomCardEffect.Apply(game);
        }
    }
}
