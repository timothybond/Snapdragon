using Snapdragon.CardOrders;
using Snapdragon.GeneticAlgorithm;

namespace Snapdragon.Runner.Experiments
{
    public class KaZarExperiment
    {
        public async Task Run()
        {
            const int Simulations = 10;
            const int MutationsPer = 100;

            var kaZarDefinition = SnapCards.ByName["Ka-Zar"];

            var withoutKaZar = new Genetics(
                [],
                SnapCards.All.Remove(kaZarDefinition),
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
                "Ka-Zar Experiment",
                DateTimeOffset.UtcNow
            );

            await populationExperiment.Run(
                withoutKaZar,
                kaZarPinned,
                "without-ka-zar",
                "with-ka-zar-pinned"
            );
        }
    }
}
