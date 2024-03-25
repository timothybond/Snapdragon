using Snapdragon.CardOrders;
using Snapdragon.GeneticAlgorithm;

namespace Snapdragon.Runner.Experiments
{
    public class KaZarSoloExperiment
    {
        public async Task Run(ISnapdragonRepositoryBuilder? repositoryBuilder = null)
        {
            const int Simulations = 10;
            const int MutationsPer = 100;

            var selfPlay = new PopulationSelfPlay(
                Guid.NewGuid(),
                "Without Ka-Zar Self-Play 128x200x40",
                DateTimeOffset.UtcNow,
                repositoryBuilder
            );

            var kaZarDefinition = SnapCards.ByName["Ka-Zar"];

            var withoutKaZar = new Genetics(
                [],
                SnapCards.All.Remove(kaZarDefinition),
                new MonteCarloSearchController(Simulations),
                MutationsPer,
                new RandomCardOrder()
            );

            await selfPlay.Run(withoutKaZar, "without-ka-zar-solo", 128, 200, 40);

            selfPlay = new PopulationSelfPlay(
                Guid.NewGuid(),
                "With Ka-Zar Pinned Self-Play 128x200x40",
                DateTimeOffset.UtcNow,
                repositoryBuilder
            );

            var kaZarPinned = new Genetics(
                [SnapCards.ByName["Ka-Zar"]],
                SnapCards.All,
                new MonteCarloSearchController(Simulations),
                MutationsPer,
                new RandomCardOrder()
            );

            await selfPlay.Run(kaZarPinned, "with-ka-zar-pinned-solo", 128, 200, 40);
        }
    }
}
