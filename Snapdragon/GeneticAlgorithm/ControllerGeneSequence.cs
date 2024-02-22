using System.Collections.Immutable;

namespace Snapdragon.GeneticAlgorithm
{
    /// <summary>
    /// A "gene sequence" that just represents what <see cref="IPlayerController"/> to use.
    ///
    /// This was mostly created to validate that the <see cref="MonteCarloSearchController"/> outperforms
    /// the <see cref="RandomPlayerController"/>, and might not really need to be supported for now,
    /// unless I end up experimenting with other controller types.
    /// </summary>
    /// <param name="Controller">The <see cref="IPlayerController"/> to use.</param>
    /// <param name="AllControllers">All possible controllers (used for mutations).</param>
    /// <param name="MutationPer">Mutation rate (given as the denominator of a fraction, 1 / [this value]).</param>
    public record ControllerGeneSequence(
        IPlayerController Controller,
        ImmutableList<IPlayerController> AllControllers,
        Guid Id,
        Guid? FirstParentId,
        Guid? SecondParentId,
        int MutationPer = 100
    ) : IGeneSequence<ControllerGeneSequence>
    {
        public ControllerGeneSequence Cross(ControllerGeneSequence other)
        {
            if (Random.Next(MutationPer) == 0)
            {
                // "Mutate" - get a random CardDefinition from all cards, instead of the normal logic
                var mutantGene = Random.Of(AllControllers);

                return this with
                {
                    Controller = mutantGene
                };
            }

            if (Random.NextBool())
            {
                return this with
                {
                    Id = Guid.NewGuid(),
                    FirstParentId = this.Id,
                    SecondParentId = other.Id
                };
            }
            else
            {
                return other with
                {
                    Id = Guid.NewGuid(),
                    FirstParentId = this.Id,
                    SecondParentId = other.Id
                };
                ;
            }
        }

        public IReadOnlyList<CardDefinition> GetCards()
        {
            return new List<CardDefinition>();
        }

        public string? GetControllerString()
        {
            return Controller.ToString();
        }

        public PlayerConfiguration GetPlayerConfiguration()
        {
            throw new NotImplementedException(
                "Cannot form a PlayerConfiguration just from a ControllerGeneSequence."
            );
        }
    }
}
