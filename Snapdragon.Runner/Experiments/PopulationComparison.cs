using CsvHelper;
using Snapdragon.GeneticAlgorithm;
using System.Globalization;

namespace Snapdragon.Runner.Experiments
{
    /// <summary>
    /// An experiment that runs against two different populations that are co-evolving,
    /// recording the top decks of both and their card distributions over time.
    /// </summary>
    public record PopulationComparison(
        Guid Id,
        string Name,
        DateTimeOffset Started,
        ISnapdragonRepositoryBuilder? RepositoryBuilder = null,
        int GamesPerDeck = 5
    ) : Experiment(Id, Name, Started, RepositoryBuilder)
    {
        public async Task Run(
            Genetics firstSchema,
            Genetics secondSchema,
            string firstName,
            string secondName,
            int deckCount = 64,
            int generations = 100
        )
        {
            var first = new Population(firstSchema, deckCount, firstName, this.Id);
            var second = new Population(secondSchema, deckCount, secondName, this.Id);

            if (RepositoryBuilder != null)
            {
                using var repository = RepositoryBuilder.Build();
                await repository.SaveExperiment(this);
                await repository.SavePopulation(first);
                await repository.SavePopulation(second);
            }

            await LogDecks(first, second);

            WriteHeaders(first);
            WriteHeaders(second);

            for (var i = 0; i < generations; i++)
            {
                (first, second) = await RunGames(first, second, GamesPerDeck);

                Log.LogBestDeck(i, first);
                Log.LogBestDeck(i, second);

                first = first.Reproduce();
                second = second.Reproduce();

                await LogDecks(first, second);

                WriteCardCounts(first);
                WriteCardCounts(second);
            }

            Console.WriteLine("Finished.");
        }

        private async Task LogDecks(Population first, Population second)
        {
            if (RepositoryBuilder == null)
            {
                return;
            }

            using var repository = RepositoryBuilder.Build();

            foreach (var firstItem in first.Items)
            {
                await repository.SaveItem(firstItem);
                await repository.AddItemToPopulation(firstItem.Id, first.Id, first.Generation);
            }

            foreach (var secondItem in second.Items)
            {
                await repository.SaveItem(secondItem);
                await repository.AddItemToPopulation(secondItem.Id, second.Id, second.Generation);
            }
        }

        private void WriteCardCounts(Population population)
        {
            using (var writer = new StreamWriter($"{population.Name}.csv", true))
            {
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    var cardCounts = population.Genetics.GetCardCounts(population.Items);

                    foreach (var cardCount in cardCounts)
                    {
                        csv.WriteField(cardCount);
                    }

                    csv.NextRecord();
                }
            }
        }

        private void WriteHeaders(Population population)
        {
            using (var writer = new StreamWriter($"{population.Name}.csv"))
            {
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    foreach (var card in population.Genetics.AllPossibleCards)
                    {
                        csv.WriteField(card.Name);
                    }

                    csv.NextRecord();
                }
            }
        }

        private async Task<(Population First, Population Second)> RunGames(
            Population first,
            Population second,
            int gamesPerDeck = 5
        )
        {
            var combinedPopulations = new List<IReadOnlyList<GeneSequence>>
            {
                first.Items,
                second.Items
            };

            var allWins = await first.Genetics.RunMixedPopulationGames(
                combinedPopulations,
                gamesPerDeck,
                RepositoryBuilder
            );

            first = first with { Wins = allWins[0] };
            second = second with { Wins = allWins[1] };

            return (first, second);
        }
    }
}
