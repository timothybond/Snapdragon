using System.Collections.Immutable;

namespace Snapdragon.CardDefinitionFilters
{
    public record CardsWithCost(ImmutableArray<int> Costs) : ICardDefinitionFilter
    {
        public CardsWithCost(params int[] costs)
            : this(ImmutableArray.Create(costs)) { }

        public bool Applies(CardDefinition cardDefinition)
        {
            return this.Costs.Contains(cardDefinition.Cost);
        }
    }
}
