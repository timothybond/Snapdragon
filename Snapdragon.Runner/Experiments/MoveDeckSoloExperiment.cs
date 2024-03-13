using Snapdragon.CardOrders;
using Snapdragon.GeneticAlgorithm;

namespace Snapdragon.Runner.Experiments
{
    public class MoveDeckSoloExperiment
    {
        public async Task Run(ISnapdragonRepositoryBuilder? repositoryBuilder = null)
        {
            const int Simulations = 20;
            const int MutationsPer = 100;

            var vultureDefinition = SnapCards.ByName["Vulture"];
            var multipleManDefinition = SnapCards.ByName["Multiple Man"];

            var selfPlay = new PopulationSelfPlay(
                Guid.NewGuid(),
                "Without Vulture/MM Self Play 200x256",
                DateTimeOffset.UtcNow,
                repositoryBuilder
            );

            var withoutVultureOrMultipleMan = new Genetics(
                [],
                SnapCards.All.Remove(vultureDefinition).Remove(multipleManDefinition),
                new MonteCarloSearchController(Simulations),
                MutationsPer,
                new RandomCardOrder()
            );

            await selfPlay.Run(
                withoutVultureOrMultipleMan,
                "without-vulture-mm-solo",
                256,
                200,
                10
            );

            selfPlay = new PopulationSelfPlay(
                Guid.NewGuid(),
                "With Vulture/MM Pinned Self Play 200x256",
                DateTimeOffset.UtcNow,
                repositoryBuilder
            );

            var vultureMultipleManPinned = new Genetics(
                [SnapCards.ByName["Vulture"], SnapCards.ByName["Multiple Man"]],
                SnapCards.All,
                new MonteCarloSearchController(Simulations),
                MutationsPer,
                new RandomCardOrder()
            );

            await selfPlay.Run(vultureMultipleManPinned, "with-vulture-mm-solo", 256, 200, 10);
        }
    }
}
