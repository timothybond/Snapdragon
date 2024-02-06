namespace Snapdragon.GeneticAlgorithm
{
    public record PartiallyFixedCardGeneSequence(
        FixedCardGeneSequence FixedCards,
        CardGeneSequence EvolvingCards,
        int MonteCarloSimulationCount
    ) : IGeneSequence<PartiallyFixedCardGeneSequence>
    {
        public PartiallyFixedCardGeneSequence Cross(PartiallyFixedCardGeneSequence other)
        {
            return new PartiallyFixedCardGeneSequence(
                FixedCards.Cross(other.FixedCards),
                EvolvingCards.Cross(other.EvolvingCards),
                MonteCarloSimulationCount
            );
        }

        public PlayerConfiguration GetPlayerConfiguration(int index)
        {
            return new PlayerConfiguration(
                $"Deck {index}",
                new Deck(FixedCards.Cards.AddRange(EvolvingCards.Cards)),
                new MonteCarloSearchController(MonteCarloSimulationCount));
        }
    }
}
