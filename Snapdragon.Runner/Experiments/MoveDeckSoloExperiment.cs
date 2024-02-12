using Snapdragon.GeneticAlgorithm;

namespace Snapdragon.Runner.Experiments
{
    public class MoveDeckSoloExperiment
    {
        public void Run()
        {
            const int Simulations = 10;
            const int MutationsPer = 100;

            var selfPlay = new PopulationSelfPlay();

            var vultureDefinition = SnapCards.ByName["Vulture"];
            var multipleManDefinition = SnapCards.ByName["Multiple Man"];

            var withoutVultureOrMultipleMan = new CardGenetics(
                SnapCards.All.Remove(vultureDefinition).Remove(multipleManDefinition),
                new MonteCarloSearchController(Simulations),
                MutationsPer,
                c => Random.Next()
            );

            selfPlay.Run(withoutVultureOrMultipleMan, "without-vulture-mm-solo", 64, 100, 10);

            var vultureMultipleManPinned = new PartiallyFixedGenetics(
                [SnapCards.ByName["Vulture"], SnapCards.ByName["Multiple Man"]],
                SnapCards.All,
                new MonteCarloSearchController(Simulations),
                MutationsPer,
                c => Random.Next()
            );

            selfPlay.Run(vultureMultipleManPinned, "with-vulture-mm-solo", 64, 100, 10);
        }
    }
}
