﻿using Snapdragon.CardOrders;
using Snapdragon.GeneticAlgorithm;

namespace Snapdragon.Runner.Experiments
{
    public class VultureExperiment
    {
        public async Task Run()
        {
            const int Simulations = 10;
            const int MutationsPer = 100;

            var general = new Genetics(
                [],
                SnapCards.All,
                new MonteCarloSearchController(Simulations),
                MutationsPer,
                new RandomCardOrder()
            );

            var vulturePinned = new Genetics(
                [SnapCards.ByName["Vulture"]],
                SnapCards.All,
                new MonteCarloSearchController(Simulations),
                MutationsPer,
                new RandomCardOrder()
            );

            var populationExperiment = new PopulationComparison(
                Guid.NewGuid(),
                "Vulture Experiment",
                DateTimeOffset.UtcNow
            );
            await populationExperiment.Run(
                general,
                vulturePinned,
                "general-for-vulture",
                "with-vulture-pinned"
            );
        }
    }
}
