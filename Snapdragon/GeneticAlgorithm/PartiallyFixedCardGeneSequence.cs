namespace Snapdragon.GeneticAlgorithm
{
    public record PartiallyFixedCardGeneSequence(
        FixedCardGeneSequence FixedCards,
        CardGeneSequence EvolvingCards
    ) : IGeneSequence<PartiallyFixedCardGeneSequence>
    {
        public PartiallyFixedCardGeneSequence Cross(PartiallyFixedCardGeneSequence other)
        {
            return new PartiallyFixedCardGeneSequence(
                FixedCards.Cross(other.FixedCards),
                EvolvingCards.Cross(other.EvolvingCards)
            );
        }
    }
}
