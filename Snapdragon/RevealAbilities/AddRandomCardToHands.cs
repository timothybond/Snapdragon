using Snapdragon.CardDefinitionFilters;

namespace Snapdragon.RevealAbilities
{
    public record AddRandomCardToHands(ICardDefinitionFilter Filter) : IRevealAbility<Location>
    {
        public AddRandomCardToHands()
            : this(new AnyCardDefinition()) { }

        public Game Activate(Game game, Location source)
        {
            foreach (var side in All.Sides)
            {
                var addRandomCardEffect = new Effects.AddRandomCardToHand(side, Filter);
                game = addRandomCardEffect.Apply(game);
            }

            return game;
        }
    }
}
