using System.Collections.Immutable;

namespace Snapdragon.GeneticAlgorithm
{
    public record PartiallyFixedGenetics(
        ImmutableList<CardDefinition> FixedCards,
        ImmutableList<CardDefinition> AllPossibleCards,
        IPlayerController Controller,
        int MutationPer = 100,
        Func<CardDefinition, int>? OrderBy = null,
        int MonteCarloSimulationCount = 5
    ) : Genetics<PartiallyFixedCardGeneSequence>
    {
        private readonly CardGenetics cardGenetics =
            new(
                AllPossibleCards.RemoveAll(FixedCards.Contains),
                Controller,
                MutationPer,
                OrderBy,
                12 - FixedCards.Count
            );

        public override PartiallyFixedCardGeneSequence GetRandomItem()
        {
            var fixedCardSequence = new FixedCardGeneSequence(FixedCards);
            var evolvingCardSequence = cardGenetics.GetRandomItem();

            return new PartiallyFixedCardGeneSequence(
                fixedCardSequence,
                evolvingCardSequence,
                MonteCarloSimulationCount
            );
        }
    }
}
