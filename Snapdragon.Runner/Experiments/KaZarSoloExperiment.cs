using Snapdragon.GeneticAlgorithm;

namespace Snapdragon.Runner.Experiments
{
    public class KaZarSoloExperiment
    {
        public void Run()
        {
            const int Simulations = 10;
            const int MutationsPer = 100;

            var selfPlay = new PopulationSelfPlay(
                Guid.NewGuid(),
                "Without Ka-Zar Self-Play",
                DateTimeOffset.UtcNow
            );

            var kaZarDefinition = SnapCards.ByName["Ka-Zar"];

            var withoutKaZar = new CardGenetics(
                SnapCards.All.Remove(kaZarDefinition),
                new MonteCarloSearchController(Simulations),
                MutationsPer,
                c => Random.Next()
            );

            selfPlay.Run(withoutKaZar, "without-ka-zar-solo", 8, 100, 2);

            selfPlay = new PopulationSelfPlay(
                Guid.NewGuid(),
                "With Ka-Zar Pinned Self-Play",
                DateTimeOffset.UtcNow
            );

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
