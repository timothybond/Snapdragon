using Snapdragon.CardOrders;
using Snapdragon.GeneticAlgorithm;

namespace Snapdragon.Runner.Experiments
{
    public class HelaExperiment
    {
        public async Task Run(ISnapdragonRepositoryBuilder? repositoryBuilder = null)
        {
            const int Simulations = 20;
            const int MutationsPer = 100;

            var helaDefinition = SnapCards.ByName["Hela"];

            var selfPlay = new PopulationSelfPlay(
                Guid.NewGuid(),
                "Hela Pinned Self Play 200x256",
                DateTimeOffset.UtcNow,
                repositoryBuilder
            );

            var helaPinned = new Genetics(
                [helaDefinition],
                SnapCards.All,
                new MonteCarloSearchController(Simulations),
                MutationsPer,
                new RandomCardOrder()
            );

            await selfPlay.Run(helaPinned, "with-hela-solo", 256, 200, 10);
        }
    }
}
