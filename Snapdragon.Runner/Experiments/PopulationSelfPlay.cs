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
        public PopulationSelfPlay(Guid Id, string Name, DateTimeOffset Started)
            : base(Id, Name, Started) { }

        public void Run<T>(
            Genetics<T> schema,
            string name,
            int deckCount = 64,
            int generations = 100,
            int gamesPerDeck = 5
        )
            where T : IGeneSequence<T>
        {
            var population = new Population<T>(schema, deckCount, name, this.Id);

            WriteHeaders(population);

            for (var i = 0; i < generations; i++)
            {
                population = RunGames(population, gamesPerDeck);

                Log.LogBestDeck(i, population);

                population = population.Reproduce();

                WriteCardCounts(population);
            }

            Console.WriteLine("Finished.");
        }

        private void WriteCardCounts<T>(Population<T> population)
            where T : IGeneSequence<T>
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

        private void WriteHeaders<T>(Population<T> population)
            where T : IGeneSequence<T>
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

        private Population<T> RunGames<T>(Population<T> population, int gamesPerDeck)
            where T : IGeneSequence<T>
        {
            var engine = new Engine(new NullLogger());

            var wins = population.Genetics.RunPopulationGames(
                population.Items.Cast<IGeneSequence>().ToList(),
                engine,
                gamesPerDeck
            );
            population = population with { Wins = wins };

            return population;
        }
    }
}
