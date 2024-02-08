using Snapdragon.GeneticAlgorithm;

namespace Snapdragon.Runner.Experiments
{
    public class VultureExperiment
    {
        public void Run()
        {
            const int Simulations = 10;
            const int MutationsPer = 100;

            var general = new CardGenetics(
                SnapCards.All,
                new MonteCarloSearchController(Simulations),
                MutationsPer,
                c => Random.Next()
            );

            var vulturePinned = new PartiallyFixedGenetics(
                [SnapCards.ByName["Vulture"]],
                SnapCards.All,
                new MonteCarloSearchController(Simulations),
                MutationsPer,
                c => Random.Next()
            );

            var populationExperiment = new PopulationComparison();
            populationExperiment.Run(general, vulturePinned, "general-for-vulture", "with-vulture-pinned");
        }
    }
}
