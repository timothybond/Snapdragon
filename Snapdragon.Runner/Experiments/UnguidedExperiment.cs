using Snapdragon.CardOrders;
using Snapdragon.GeneticAlgorithm;

namespace Snapdragon.Runner.Experiments
{
    public class UnguidedExperiment
    {
        public async Task Run(ISnapdragonRepositoryBuilder? repositoryBuilder = null)
        {
            const int Simulations = 10;
            const int MutationsPer = 100;

            var selfPlay = new PopulationSelfPlay(
                Guid.NewGuid(),
                "Unguided Self-Play 128x200x40",
                DateTimeOffset.UtcNow,
                repositoryBuilder
            );

            var genetics = new Genetics(
                [],
                SnapCards.All,
                new MonteCarloSearchController(Simulations),
                MutationsPer,
                new RandomCardOrder()
            );

            await selfPlay.Run(genetics, "unguided-population", 128, 200, 40);
        }
    }
}
