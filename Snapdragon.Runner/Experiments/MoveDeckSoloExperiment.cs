using Snapdragon.CardOrders;
using Snapdragon.GeneticAlgorithm;

namespace Snapdragon.Runner.Experiments
{
    public class MoveDeckSoloExperiment
    {
        public void Run()
        {
            const int Simulations = 10;
            const int MutationsPer = 100;

            var vultureDefinition = SnapCards.ByName["Vulture"];
            var multipleManDefinition = SnapCards.ByName["Multiple Man"];

            var selfPlay = new PopulationSelfPlay(
                Guid.NewGuid(),
                "Without Vulture/MM Self Play",
                DateTimeOffset.UtcNow
            );

            var withoutVultureOrMultipleMan = new CardGenetics(
                SnapCards.All.Remove(vultureDefinition).Remove(multipleManDefinition),
                new MonteCarloSearchController(Simulations),
                MutationsPer,
                new RandomCardOrder()
            );

            selfPlay.Run(withoutVultureOrMultipleMan, "without-vulture-mm-solo", 64, 100, 10);

            selfPlay = new PopulationSelfPlay(
                Guid.NewGuid(),
                "With Vulture/MM Pinned Self Play",
                DateTimeOffset.UtcNow
            );

            var vultureMultipleManPinned = new PartiallyFixedGenetics(
                [SnapCards.ByName["Vulture"], SnapCards.ByName["Multiple Man"]],
                SnapCards.All,
                new MonteCarloSearchController(Simulations),
                MutationsPer,
                new RandomCardOrder()
            );

            selfPlay.Run(vultureMultipleManPinned, "with-vulture-mm-solo", 64, 100, 10);
        }
    }
}
