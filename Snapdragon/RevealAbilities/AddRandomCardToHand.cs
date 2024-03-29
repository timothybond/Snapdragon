using Snapdragon.CardDefinitionFilters;

namespace Snapdragon.RevealAbilities
{
    public record AddRandomCardToHand(ICardDefinitionFilter Filter) : IRevealAbility<ICard>
    {
        public AddRandomCardToHand()
            : this(new AnyCardDefinition()) { }

        public Game Activate(Game game, ICard source)
        {
            var addRandomCardEffect = new Effects.AddRandomCardToHand(source.Side, Filter);
            return addRandomCardEffect.Apply(game);
        }
    }
}
