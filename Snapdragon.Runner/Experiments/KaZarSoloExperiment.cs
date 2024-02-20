using Snapdragon.GeneticAlgorithm;

namespace Snapdragon.Runner.Experiments
{
    public class KaZarSoloExperiment
    {
        public void Run()
        {
            const int Simulations = 10;
            const int MutationsPer = 100;

            var selfPlay = new PopulationSelfPlay();

            var kaZarDefinition = SnapCards.ByName["Ka-Zar"];

            var withoutKaZar = new CardGenetics(
                SnapCards.All.Remove(kaZarDefinition),
                new MonteCarloSearchController(Simulations),
                MutationsPer,
                c => Random.Next()
            );

            selfPlay.Run(withoutKaZar, "without-ka-zar-solo", 8, 100, 2);

            var kaZarPinned = new PartiallyFixedGenetics(
                [SnapCards.ByName["Ka-Zar"]],
                SnapCards.All,
                new MonteCarloSearchController(Simulations),
                MutationsPer,
                c => Random.Next()
            );

            selfPlay.Run(kaZarPinned, "with-ka-zar-pinned-solo", 64, 100, 10);
        }
    }
}
