using System.Globalization;
using CsvHelper;
using Snapdragon.GeneticAlgorithm;

namespace Snapdragon.Runner.Experiments
{
    /// <summary>
    /// An experiment that runs against two different populations that are co-evolving,
    /// recording the top decks of both and their card distributions over time.
    /// </summary>
    public class PopulationComparison
    {
        public void Run<TFirst, TSecond>(
            Genetics<TFirst> firstSchema,
            Genetics<TSecond> secondSchema,
            string firstName,
            string secondName,
            int deckCount = 32,
            int generations = 50
        )
            where TFirst : IGeneSequence<TFirst>
            where TSecond : IGeneSequence<TSecond>
        {
            var first = new Population<TFirst>(firstSchema, deckCount, firstName);
            var second = new Population<TSecond>(secondSchema, deckCount, secondName);

            WriteHeaders(first);
            WriteHeaders(second);

            for (var i = 0; i < generations; i++)
            {
                (first, second) = RunGames(first, second);

                Log.LogBestDeck(i, first);
                Log.LogBestDeck(i, second);

                first = first.Reproduce();
                second = second.Reproduce();

                WriteCardCounts(first);
                WriteCardCounts(second);
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

        private (Population<TFirst> First, Population<TSecond> Second) RunGames<TFirst, TSecond>(
            Population<TFirst> first,
            Population<TSecond> second,
            int gamesPerDeck = 5
        )
            where TFirst : IGeneSequence<TFirst>
            where TSecond : IGeneSequence<TSecond>
        {
            var engine = new Engine(new NullLogger());

            var combinedPopulations = new List<IReadOnlyList<IGeneSequence>>
            {
                first.Items.Cast<IGeneSequence>().ToList(),
                second.Items.Cast<IGeneSequence>().ToList()
            };

            var allWins = first.Genetics.RunMixedPopulationGames(
                combinedPopulations,
                engine,
                gamesPerDeck
            );

            first = first with { Wins = allWins[0] };
            second = second with { Wins = allWins[1] };

            return (first, second);
        }
    }
}
