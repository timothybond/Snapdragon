using Snapdragon.CardOrders;
using Snapdragon.GeneticAlgorithm;

namespace Snapdragon.Runner.Experiments
{
    public class KaZarHazmatExperiment
    {
        public async Task Run(ISnapdragonRepositoryBuilder? repositoryBuilder = null)
        {
            const int Simulations = 1;
            const int MutationsPer = 100;

            var hazmatPinned = new Genetics(
                [SnapCards.ByName["Hazmat"]],
                SnapCards.All,
                new MonteCarloSearchController(Simulations),
                MutationsPer,
                new RandomCardOrder()
            );

            var kaZarPinned = new Genetics(
                [SnapCards.ByName["Ka-Zar"]],
                SnapCards.All,
                new MonteCarloSearchController(Simulations),
                MutationsPer,
                new RandomCardOrder()
            );

            var populationExperiment = new PopulationComparison(
                Guid.NewGuid(),
                "Ka-Zar vs. Hazmat Experiment 128x200x40",
                DateTimeOffset.UtcNow,
                repositoryBuilder
            );

            await populationExperiment.Run(
                hazmatPinned,
                kaZarPinned,
                "with-hazmat-pinned",
                "with-ka-zar-pinned",
                32,
                1
            );
        }
    }
}
