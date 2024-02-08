using Snapdragon.GeneticAlgorithm;

namespace Snapdragon.Runner.Experiments
{
    public class KaZarExperiment
    {
        public void Run()
        {
            const int Simulations = 10;
            const int MutationsPer = 100;

            var kaZarDefinition = SnapCards.ByName["Ka-Zar"];

            var withoutKaZar = new CardGenetics(
                SnapCards.All.Remove(kaZarDefinition),
                new MonteCarloSearchController(Simulations),
                MutationsPer,
                c => Random.Next()
            );

            var kaZarPinned = new PartiallyFixedGenetics(
                [SnapCards.ByName["Ka-Zar"]],
                SnapCards.All,
                new MonteCarloSearchController(Simulations),
                MutationsPer,
                c => Random.Next()
            );

            var populationExperiment = new PopulationComparison();
            populationExperiment.Run(
                withoutKaZar,
                kaZarPinned,
                "without-ka-zar",
                "with-ka-zar-pinned"
            );
        }
    }
}
