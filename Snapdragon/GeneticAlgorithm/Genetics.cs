using System.Collections.Immutable;

namespace Snapdragon.GeneticAlgorithm
{
    public record Genetics(
        ImmutableList<CardDefinition> FixedCards,
        ImmutableList<CardDefinition> AllPossibleCards,
        IPlayerController Controller,
        int MutationPer,
        ICardOrder OrderBy
    )
    {
        public string? GetControllerString()
        {
            return Controller.ToString();
        }

        public ImmutableList<CardDefinition> GetFixedCards()
        {
            return FixedCards;
        }

        public GeneSequence GetRandomItem()
        {
            var fixedCardNames = new HashSet<string>(FixedCards.Select(fc => fc.Name));

            var evolvingCards = AllPossibleCards
                .Where(c => !fixedCardNames.Contains(c.Name))
                .OrderBy(c => Random.Next())
                .Take(12 - FixedCards.Count)
                .ToImmutableList();

            return new GeneSequence(
                FixedCards,
                evolvingCards,
                this,
                Controller,
                Guid.NewGuid(),
                null,
                null
            );
        }

        protected IReadOnlyList<CardDefinition> GetCards(GeneSequence item)
        {
            return item.FixedCards.Concat(item.EvolvingCards).ToList();
        }

        /// <summary>
        /// Gets the next generation of the given population, by reproducing the top half in random pairs.
        /// </summary>
        /// <param name="population">The prior generation.</param>
        /// <param name="wins">The number of wins for each item in the prior generation.</param>
        /// <param name="pinTopX">
        /// If greater than 0, the top [this many] items are copied into the next generation,
        /// to avoid losing what might be particularly good decks.</param>
        /// <returns></returns>
        public IReadOnlyList<GeneSequence> ReproducePopulation(
            IReadOnlyList<GeneSequence> population,
            IReadOnlyList<int> wins,
            int pinTopX = 0
        )
        {
            var itemsByDescendingWins = population
                .Select((item, index) => (Item: item, Wins: wins[index]))
                .OrderByDescending(itemsAndWins => itemsAndWins.Wins)
                .Select(itemsAndWins => itemsAndWins.Item)
                .ToList();

            var survivingItems = itemsByDescendingWins
                .Take(itemsByDescendingWins.Count / 2)
                .ToList();

            var pairs = GetRandomPairs(survivingItems.Count, 4);

            var nextGeneration = new List<GeneSequence>();

            if (pinTopX > 0)
            {
                nextGeneration.AddRange(survivingItems.Take(pinTopX));
                pairs = pairs.Skip(pinTopX).ToList();
            }

            foreach (var pair in pairs)
            {
                nextGeneration.Add(survivingItems[pair.First].Cross(survivingItems[pair.Second]));
            }

            return nextGeneration;
        }

        /// <summary>
        /// Runs the specified number of games with each <see cref="Deck"/>, pairing them randomly against each other.
        ///
        /// Members of the different populations are all intermixed for the purposes of competition,
        /// although it is expected that they will reproduce separately.
        /// </summary>
        /// <returns>The number of wins per each <see cref="Deck"/>.</returns>
        public async Task<IReadOnlyList<IReadOnlyList<int>>> RunMixedPopulationGames(
            IReadOnlyList<IReadOnlyList<GeneSequence>> populations,
            int gamesPerDeck = 5,
            ISnapdragonRepositoryBuilder? repositoryBuilder = null
        )
        {
            if (populations.Count == 0)
            {
                return new List<IReadOnlyList<int>>();
            }

            for (var i = 1; i < populations.Count; i++)
            {
                if (populations[i].Count != populations[0].Count)
                {
                    throw new InvalidOperationException(
                        "All populations must have the same number of items."
                    );
                }
            }

            var combinedPopulation = populations.SelectMany(p => p).ToList();
            var wins = await RunPopulationGames(
                combinedPopulation,
                gamesPerDeck,
                repositoryBuilder
            );

            var winsByPopulation = new List<IReadOnlyList<int>>();

            for (var i = 0; i < populations.Count; i++)
            {
                winsByPopulation.Add(
                    wins.Skip(i * populations[0].Count).Take(populations[0].Count).ToList()
                );
            }

            return winsByPopulation;
        }

        /// <summary>
        /// Runs the specified number of games with each <see cref="Deck"/>, pairing them randomly against each other.
        /// </summary>
        /// <returns>The number of wins per each <see cref="Deck"/>.</returns>
        public async Task<IReadOnlyList<int>> RunPopulationGames(
            IReadOnlyList<GeneSequence> population,
            int gamesPerDeck = 5,
            ISnapdragonRepositoryBuilder? repositoryBuilder = null,
            Guid? experimentId = null,
            int? generation = null
        )
        {
            var pairs = GetRandomPairs(population.Count, gamesPerDeck);

            var totalGamesWon = population.Select(d => 0).ToList();

            var repository = repositoryBuilder?.Build();

            try
            {
                // TODO: Re-enable parallelism
                //await pairs.ForEachAsync(
                //    async pair =>
                //    {
                //        return await PlayGameAndGetWinnerIndex(
                //            population,
                //            pair,
                //            repository,
                //            experimentId,
                //            generation
                //        );
                //    },
                //    (pair, winner) =>
                //    {
                //        if (winner >= 0)
                //        {
                //            lock (totalGamesWon)
                //            {
                //                totalGamesWon[winner] += 1;
                //            }
                //        }
                //    }
                //);

                // TODO: Use this if you need to test with a single thread
                foreach (var pair in pairs)
                {
                    var winner = await PlayGameAndGetWinnerIndex(
                        population,
                        pair,
                        repository,
                        experimentId,
                        generation
                    );

                    if (winner >= 0)
                    {
                        lock (totalGamesWon)
                        {
                            totalGamesWon[winner] += 1;
                        }
                    }
                }

                return totalGamesWon;
            }
            finally
            {
                if (repository != null)
                {
                    repository.Dispose();
                }
            }
        }

        private async Task<int> PlayGameAndGetWinnerIndex(
            IReadOnlyList<GeneSequence> population,
            (int First, int Second) pair,
            ISnapdragonRepository? repository = null,
            Guid? experimentId = null,
            int? generation = null
        )
        {
            var engine = new Engine(new NullLogger());

            var topIndex = pair.First;
            var bottomIndex = pair.Second;

            var topPlayerConfig = population[topIndex].GetPlayerConfiguration();
            var bottomPlayerConfig = population[bottomIndex].GetPlayerConfiguration();

            var game = engine.CreateGame(
                topPlayerConfig,
                bottomPlayerConfig,
                repository: repository,
                experimentId: experimentId,
                generation: generation,
                // TODO: Remove these once locations are in good shape
                leftLocationName: "Ruins",
                middleLocationName: "Ruins",
                rightLocationName: "Ruins"
            );

            game = game.PlayGame();

            await game.Logger.LogFinishedGame(game);

            // For the moment we will only count victories, not ties.
            var winner = game.GetLeader();
            if (winner != null)
            {
                switch (winner)
                {
                    case Side.Top:
                        return topIndex;
                    case Side.Bottom:
                        return bottomIndex;
                    default:
                        throw new NotImplementedException();
                }
            }

            return -1;
        }

        public List<(int First, int Second)> GetRandomPairs(int populationSize, int pairsPerItem)
        {
            var pairs = new List<(int First, int Second)>();

            var indexesRemaining = new List<int>();
            for (var i = 0; i < populationSize; i++)
            {
                indexesRemaining.Add(i);
            }
            var totalInclusions = indexesRemaining.Select(i => 0).ToList();

            while (indexesRemaining.Count > 0)
            {
                var firstIndex = indexesRemaining[0];

                // TODO: Maybe this this? It's possible to have only one item left,
                // so it will only pair with itself at that point.
                var secondIndex =
                    indexesRemaining.Count == 1
                        ? indexesRemaining[0]
                        : indexesRemaining
                            .Skip(1 + Random.Next(indexesRemaining.Count - 1))
                            .First();

                pairs.Add((firstIndex, secondIndex));

                foreach (var index in new[] { firstIndex, secondIndex })
                {
                    totalInclusions[index] += 1;
                    if (totalInclusions[index] >= pairsPerItem)
                    {
                        indexesRemaining.Remove(index);
                    }
                }
            }

            return pairs;
        }

        /// <summary>
        /// Gets a random member of the population, based on the overall possible genes.
        /// </summary>
        public IReadOnlyList<GeneSequence> GetRandomPopulation(int itemCount)
        {
            if (itemCount % 4 != 0)
            {
                throw new ArgumentException(
                    $"Item count must be divisible by 4 (was {itemCount})."
                );
            }

            var population = new List<GeneSequence>();

            for (var i = 0; i < itemCount; i++)
            {
                population.Add(this.GetRandomItem());
            }

            return population;
        }

        public List<int> GetCardCounts(IReadOnlyList<GeneSequence> population)
        {
            var cardLists = population.Select(item => GetCards(item));

            return AllPossibleCards
                .Select(snapCard =>
                    cardLists.Count(i => i.Any(c => string.Equals(c.Name, snapCard.Name)))
                )
                .ToList();
        }
    }
}
