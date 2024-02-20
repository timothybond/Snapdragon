using System.Collections.Immutable;

namespace Snapdragon.GeneticAlgorithm
{
    /// <summary>
    /// A variation on <see cref="CardGeneSequence"/> where the genes are
    /// actually unchanging.
    ///
    /// Intended as part of an object with some "pinned" or "guaranteed" cards,
    /// and the rest allowed to change over time.
    /// </summary>
    public record FixedCardGeneSequence(ImmutableList<CardDefinition> Cards)
        : IGeneSequence<FixedCardGeneSequence>
    {
        public FixedCardGeneSequence Cross(FixedCardGeneSequence other)
        {
            // TODO: Consider a sanity check that the other FixedCardGeneSequence is equal
            return this;
        }

        public IReadOnlyList<CardDefinition> GetCards()
        {
            return this.Cards;
        }

        public PlayerConfiguration GetPlayerConfiguration()
        {
            throw new NotImplementedException(
                "Cannot form a PlayerConfiguration just from a FixedCardGeneSequence."
            );
        }
    }
}
