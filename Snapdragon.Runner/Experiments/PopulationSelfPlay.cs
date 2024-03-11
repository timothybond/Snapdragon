using System.Globalization;
using CsvHelper;
using Snapdragon.GeneticAlgorithm;

namespace Snapdragon.Runner.Experiments
{
    /// <summary>
    /// An experiment that runs against two different populations that are co-evolving,
    /// recording the top decks of both and their card distributions over time.
    /// </summary>
    public record PopulationSelfPlay : Experiment
    {
        public PopulationSelfPlay(
            Guid Id,
            string Name,
            DateTimeOffset Started,
            ISnapdragonRepositoryBuilder? RepositoryBuilder = null
        )
            : base(Id, Name, Started, RepositoryBuilder) { }

        public async Task Run(
            Genetics schema,
            string name,
            int deckCount = 64,
            int generations = 100,
            int gamesPerDeck = 5
        )
        {
            var population = new Population(schema, deckCount, name, this.Id);

            if (RepositoryBuilder != null)
            {
                using var repository = RepositoryBuilder.Build();
                await repository.SaveExperiment(this);
                await repository.SavePopulation(population);
            }

            await LogDecks(population);

            WriteHeaders(population);

            for (var i = 0; i < generations; i++)
            {
                population = await RunGames(population, gamesPerDeck);

                Log.LogBestDeck(i, population);

                population = population.Reproduce();
                await LogDecks(population);

                WriteCardCounts(population);
            }

            Console.WriteLine("Finished.");
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

        private async Task<Population> RunGames(Population population, int gamesPerDeck)
        {
            var wins = await population.Genetics.RunPopulationGames(
                population.Items,
                gamesPerDeck,
                RepositoryBuilder,
                Id,
                population.Generation
            );
            population = population with { Wins = wins };

            return population;
        }

        private async Task LogDecks(Population population)
        {
            if (RepositoryBuilder == null)
            {
                return;
            }

            foreach (var item in population.Items)
            {
                using var repository = RepositoryBuilder.Build();
                await repository.SaveItem(item);
                await repository.AddItemToPopulation(item.Id, population.Id, population.Generation);
            }
        }
    }
}
