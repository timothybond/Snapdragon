﻿using System.Globalization;
using CsvHelper;
using Snapdragon.GeneticAlgorithm;

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
        public async Task Run<TFirst, TSecond>(
            Genetics<TFirst> firstSchema,
            Genetics<TSecond> secondSchema,
            string firstName,
            string secondName,
            int deckCount = 64,
            int generations = 100
        )
            where TFirst : IGeneSequence<TFirst>
            where TSecond : IGeneSequence<TSecond>
        {
            var first = new Population<TFirst>(firstSchema, deckCount, firstName, this.Id);
            var second = new Population<TSecond>(secondSchema, deckCount, secondName, this.Id);

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

        private async Task LogDecks<TFirst, TSecond>(
            Population<TFirst> first,
            Population<TSecond> second
        )
            where TFirst : IGeneSequence<TFirst>
            where TSecond : IGeneSequence<TSecond>
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

        private async Task<(Population<TFirst> First, Population<TSecond> Second)> RunGames<
            TFirst,
            TSecond
        >(Population<TFirst> first, Population<TSecond> second, int gamesPerDeck = 5)
            where TFirst : IGeneSequence<TFirst>
            where TSecond : IGeneSequence<TSecond>
        {
            var combinedPopulations = new List<IReadOnlyList<IGeneSequence>>
            {
                first.Items.Cast<IGeneSequence>().ToList(),
                second.Items.Cast<IGeneSequence>().ToList()
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
