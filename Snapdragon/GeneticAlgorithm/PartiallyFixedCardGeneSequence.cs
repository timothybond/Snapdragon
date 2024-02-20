namespace Snapdragon.GeneticAlgorithm
{
    public record PartiallyFixedCardGeneSequence(
        FixedCardGeneSequence FixedCards,
        CardGeneSequence EvolvingCards,
        int MonteCarloSimulationCount,
        Guid Id
    ) : IGeneSequence<PartiallyFixedCardGeneSequence>
    {
        public PartiallyFixedCardGeneSequence Cross(PartiallyFixedCardGeneSequence other)
        {
            return new PartiallyFixedCardGeneSequence(
                FixedCards.Cross(other.FixedCards),
                EvolvingCards.Cross(other.EvolvingCards),
                MonteCarloSimulationCount,
                Guid.NewGuid()
            );
        }

        public IReadOnlyList<CardDefinition> GetCards()
        {
            return this.FixedCards.Cards.Concat(this.EvolvingCards.Cards).ToList();
        }

        public PlayerConfiguration GetPlayerConfiguration()
        {
            return new PlayerConfiguration(
                Id.ToString(),
                new Deck(FixedCards.Cards.AddRange(EvolvingCards.Cards), Id),
                new MonteCarloSearchController(MonteCarloSimulationCount)
            );
        }
    }
}
