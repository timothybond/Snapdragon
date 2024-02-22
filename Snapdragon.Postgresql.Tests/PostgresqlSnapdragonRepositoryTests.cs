using System.Collections.Immutable;
using Dapper;
using Npgsql;
using Snapdragon.CardOrders;
using Snapdragon.GeneticAlgorithm;
using Testcontainers.PostgreSql;

namespace Snapdragon.Postgresql.Tests
{
    public class PostgresqlSnapdragonRepositoryTests
    {
        private const int Port = 9037; // Just a random number

        private PostgreSqlContainer _container;
        private string _connectionString = string.Empty;

        private PostgresqlSnapdragonRepository _repository;

        #region Setup / Teardown

        [OneTimeSetUp]
        public async Task InitializePostgresql()
        {
            _container = new PostgreSqlBuilder().WithPortBinding(Port).Build();

            await _container.StartAsync();

            _connectionString = _container.GetConnectionString();
            _repository = new(_connectionString);

            using var datasource = NpgsqlDataSource.Create(_connectionString);
            using (var connection = datasource.OpenConnection())
            {
                // TODO: Find a more elegant solution for this
                var sql = File.ReadAllText("../../../../Snapdragon.Postgresql/CreateDatabase.sql");

                await connection.ExecuteAsync(sql);
            }
        }

        [OneTimeTearDown]
        public async Task TearDownPostgresql()
        {
            try
            {
                _repository.Dispose();
            }
            finally
            {
                await _container.StopAsync();
                await _container.DisposeAsync();
            }
        }

        [SetUp]
        public async Task Setup()
        {
            await ClearRepository();
            await SaveAllCardDefinitions();
        }

        #endregion

        #region Experiment Tests

        [Test]
        public async Task SaveExperiment_ReturnedFromGet()
        {
            var experiment = new GeneticAlgorithm.Experiment(
                Guid.NewGuid(),
                "Test Saved Experiment",
                TruncatedNow()
            );

            await _repository.SaveExperiment(experiment);

            var savedExperiment = await _repository.GetExperiment(experiment.Id);

            Assert.That(savedExperiment, Is.EqualTo(experiment));
        }

        [Test]
        public async Task SaveExperiment_ReturnedFromGetAll()
        {
            var experiment = new GeneticAlgorithm.Experiment(
                Guid.NewGuid(),
                "Test Saved Experiment In List",
                TruncatedNow()
            );

            await _repository.SaveExperiment(experiment);

            var savedExperiments = await _repository.GetExperiments();

            Assert.That(savedExperiments.Count, Is.EqualTo(1));
            Assert.That(savedExperiments[0], Is.EqualTo(experiment));
        }

        [Test]
        public async Task SaveExperiment_Existing_UpdatesName()
        {
            var experiment = new Experiment(Guid.NewGuid(), "Test Original Name", TruncatedNow());
            await _repository.SaveExperiment(experiment);

            experiment = experiment with { Name = "Test New Name" };
            await _repository.SaveExperiment(experiment);

            var savedExperiment = await _repository.GetExperiment(experiment.Id);

            Assert.That(savedExperiment?.Name, Is.EqualTo("Test New Name"));
        }

        [Test]
        public async Task SaveExperiment_Existing_UpdatesTimestamp()
        {
            var initialTime = TruncatedNow();
            var experiment = new GeneticAlgorithm.Experiment(
                Guid.NewGuid(),
                "Test With Different Created Timestamps",
                initialTime
            );
            await _repository.SaveExperiment(experiment);

            Thread.Sleep(5);
            var newTime = TruncatedNow();
            Assert.That(initialTime, Is.Not.EqualTo(newTime));

            experiment = experiment with { Started = newTime };
            await _repository.SaveExperiment(experiment);

            var savedExperiment = await _repository.GetExperiment(experiment.Id);
            Assert.That(savedExperiment?.Started, Is.EqualTo(newTime));
        }

        [Test]
        public async Task DeleteExperiment_NoLongerRetrievable()
        {
            var experiment = new GeneticAlgorithm.Experiment(
                Guid.NewGuid(),
                "Test Deleted Experiment",
                TruncatedNow()
            );

            await _repository.SaveExperiment(experiment);
            await _repository.DeleteExperiment(experiment.Id);

            var savedExperiment = await _repository.GetExperiment(experiment.Id);
            Assert.That(savedExperiment, Is.Null);

            var savedExperiments = await _repository.GetExperiments();
            Assert.That(savedExperiments, Is.Empty);
        }

        [Test]
        public async Task GetNonexistentExperiment_ReturnsNull()
        {
            var noExperiment = await _repository.GetExperiment(Guid.Empty);

            Assert.That(noExperiment, Is.Null);
        }

        #endregion

        #region Card Definition Tests

        [Test]
        [TestCase("Test Card 1", 1, 3)]
        [TestCase("Test Card 2", 5, -1)]
        public async Task SaveCardDefinition_ReturnedFromGet(string name, int cost, int power)
        {
            var card = new CardDefinition(name, cost, power);
            await _repository.SaveCardDefinition(card);

            var savedCard = await _repository.GetCardDefinition(card.Name);

            Assert.That(savedCard, Is.EqualTo(card));
        }

        [Test]
        [TestCase("Test Card 3", 2, 2)]
        [TestCase("Test Card 4", 6, 12)]
        public async Task SaveCardDefinition_ReturnedFromGetAll(string name, int cost, int power)
        {
            var card = new CardDefinition(name, cost, power);
            await _repository.SaveCardDefinition(card);

            var savedCards = (await _repository.GetCardDefinitions())
                .Where(card => string.Equals(card.Name, name))
                .ToList();

            Assert.That(savedCards.Count, Is.EqualTo(1));
            Assert.That(savedCards[0], Is.EqualTo(card));
        }

        [Test]
        [TestCase("Test Updated Card 1", 1, 3, 2)]
        [TestCase("Test Updated Card 2", 5, -1, 4)]
        public async Task SaveCardDefinition_Existing_UpdatesCost(
            string name,
            int cost,
            int power,
            int newCost
        )
        {
            var card = new CardDefinition(name, cost, power);
            await _repository.SaveCardDefinition(card);

            card = card with { Cost = newCost };
            await _repository.SaveCardDefinition(card);

            var savedCard = await _repository.GetCardDefinition(card.Name);
            Assert.That(savedCard?.Cost, Is.EqualTo(newCost));
        }

        [Test]
        [TestCase("Test Updated Card 1", 1, 3, 2)]
        [TestCase("Test Updated Card 2", 5, -1, 4)]
        public async Task SaveCardDefinition_Existing_UpdatesPower(
            string name,
            int cost,
            int power,
            int newPower
        )
        {
            var card = new CardDefinition(name, cost, power);
            await _repository.SaveCardDefinition(card);

            card = card with { Power = newPower };
            await _repository.SaveCardDefinition(card);

            var savedCard = await _repository.GetCardDefinition(card.Name);
            Assert.That(savedCard?.Power, Is.EqualTo(newPower));
        }

        [Test]
        [TestCase("Test Deleted Card 1", 1, 0)]
        [TestCase("Test Deleted Card 2", 0, 1)]
        public async Task DeleteCardDefinition_NoLongerRetrievable(string name, int cost, int power)
        {
            var card = new CardDefinition(name, cost, power);
            await _repository.SaveCardDefinition(card);

            await _repository.DeleteCardDefinition(card.Name);

            var savedCard = await _repository.GetCardDefinition(card.Name);
            Assert.That(savedCard, Is.Null);

            var savedCards = await _repository.GetCardDefinitions();
            Assert.That(savedCards.Where(c => string.Equals(c.Name, name)).Count(), Is.EqualTo(0));
        }

        #endregion

        #region Population Tests

        [Test]
        [TestCase(20)]
        [TestCase(100)]
        [TestCase(200)]
        public async Task SavePopulation_GetsBackSameMutationRate(int mutationPer)
        {
            var experimentId = await SaveExperiment("Test Mutation Rate Experiment");

            var population = new Population<CardGeneSequence>(
                new CardGenetics(
                    SnapCards.All,
                    new RandomPlayerController(),
                    mutationPer,
                    new ExistingCardOrder()
                ),
                0,
                "Test Pop Mutation Rate",
                experimentId
            );

            await _repository.SavePopulation(population);

            var savedPopulation = await _repository.GetPopulation<CardGeneSequence>(population.Id);

            Assert.That(savedPopulation, Is.Not.Null);
            Assert.That(savedPopulation.Genetics.MutationPer, Is.EqualTo(mutationPer));
        }

        [Test]
        public async Task SavePopulation_RandomController_GetsBackSameController()
        {
            var experimentId = await SaveExperiment("Test Random Controller Experiment");

            var population = new Population<CardGeneSequence>(
                new CardGenetics(
                    SnapCards.All,
                    new RandomPlayerController(),
                    25,
                    new ExistingCardOrder()
                ),
                0,
                "Test Pop Random Controller",
                experimentId
            );

            await _repository.SavePopulation(population);

            var savedPopulation = await _repository.GetPopulation<CardGeneSequence>(population.Id);

            Assert.That(savedPopulation, Is.Not.Null);
            Assert.That(
                savedPopulation.Genetics.GetControllerString(),
                Is.EqualTo(population.Genetics.GetControllerString())
            );
        }

        [Test]
        [TestCase(1)]
        [TestCase(5)]
        [TestCase(20)]
        public async Task SavePopulation_MonteCarloController_GetsBackSameController(
            int simulationCount
        )
        {
            var experimentId = await SaveExperiment("Test Monte Carlo Experiment");

            var population = new Population<CardGeneSequence>(
                new CardGenetics(
                    SnapCards.All,
                    new MonteCarloSearchController(simulationCount),
                    25,
                    new ExistingCardOrder()
                ),
                0,
                "Test Pop Monte Carlo Controller",
                experimentId
            );

            await _repository.SavePopulation(population);

            var savedPopulation = await _repository.GetPopulation<CardGeneSequence>(population.Id);

            Assert.That(savedPopulation, Is.Not.Null);
            Assert.That(
                savedPopulation.Genetics.GetControllerString(),
                Is.EqualTo(population.Genetics.GetControllerString())
            );
        }

        [Test]
        public async Task SavePopulation_RandomOrder_GetsBackSameOrderBy()
        {
            var experimentId = await SaveExperiment("Test Random Order Experiment");

            var population = new Population<CardGeneSequence>(
                new CardGenetics(
                    SnapCards.All,
                    new RandomPlayerController(),
                    25,
                    new RandomCardOrder()
                ),
                0,
                "Test Pop Random Order",
                experimentId
            );

            await _repository.SavePopulation(population);

            var savedPopulation = await _repository.GetPopulation<CardGeneSequence>(population.Id);

            Assert.That(savedPopulation, Is.Not.Null);
            Assert.That(savedPopulation.Genetics.OrderBy, Is.TypeOf<RandomCardOrder>());
        }

        [Test]
        public async Task SavePopulation_ExistingOrder_GetsBackSameOrderBy()
        {
            var experimentId = await SaveExperiment("Test Existing Order Experiment");

            var population = new Population<CardGeneSequence>(
                new CardGenetics(
                    SnapCards.All,
                    new RandomPlayerController(),
                    25,
                    new ExistingCardOrder()
                ),
                0,
                "Test Pop Existing Order",
                experimentId
            );

            await _repository.SavePopulation(population);

            var savedPopulation = await _repository.GetPopulation<CardGeneSequence>(population.Id);

            Assert.That(savedPopulation, Is.Not.Null);
            Assert.That(savedPopulation.Genetics.OrderBy, Is.TypeOf<ExistingCardOrder>());
        }

        [Test]
        [TestCase] // Deliberately empty
        [TestCase("Ant Man", "Misty Knight")]
        [TestCase("Iron Man", "Blade", "Hulk")]
        public async Task SavePopulation_SpecificCards_GetsBackCards(params string[] cardNames)
        {
            var cards = cardNames.Select(n => SnapCards.ByName[n]).ToImmutableList();
            var experimentId = await SaveExperiment("Test Specific Cards Experiment");

            var population = new Population<CardGeneSequence>(
                new CardGenetics(cards, new RandomPlayerController(), 25, new ExistingCardOrder()),
                0,
                "Test Pop Specific Cards",
                experimentId
            );

            await _repository.SavePopulation(population);

            var savedPopulation = await _repository.GetPopulation<CardGeneSequence>(population.Id);

            Assert.That(savedPopulation, Is.Not.Null);
            Assert.That(savedPopulation.Genetics.AllPossibleCards.Count, Is.EqualTo(cards.Count));

            foreach (var card in cards)
            {
                Assert.That(savedPopulation.Genetics.AllPossibleCards, Contains.Item(card));
            }
        }

        // Note, can't have an empty one here, because a
        // partially-fixed sequence must have some fixed cards
        [Test]
        [TestCase("Ant Man", "Misty Knight")]
        [TestCase("Iron Man", "Blade", "Hulk")]
        public async Task SavePopulation_FixedCards_GetsBackCards(params string[] cardNames)
        {
            var cards = cardNames.Select(n => SnapCards.ByName[n]).ToImmutableList();
            var experimentId = await SaveExperiment("Test Fixed Cards Experiment");

            var population = new Population<PartiallyFixedCardGeneSequence>(
                new PartiallyFixedGenetics(
                    cards,
                    SnapCards.All,
                    new RandomPlayerController(),
                    25,
                    new ExistingCardOrder()
                ),
                0,
                "Test Pop Fixed Cards",
                experimentId
            );

            await _repository.SavePopulation(population);

            var savedPopulation = await _repository.GetPopulation<PartiallyFixedCardGeneSequence>(
                population.Id
            );

            Assert.That(savedPopulation, Is.Not.Null);

            var fixedCards = savedPopulation.Genetics.GetFixedCards();
            Assert.That(fixedCards.Count, Is.EqualTo(cards.Count));
            foreach (var card in cards)
            {
                Assert.That(fixedCards, Contains.Item(card));
            }

            // Also validate that the list of all cards is correct (should be unaltered)
            Assert.That(
                savedPopulation.Genetics.AllPossibleCards.Count,
                Is.EqualTo(SnapCards.All.Count)
            );
            foreach (var anyCard in SnapCards.All)
            {
                Assert.That(savedPopulation.Genetics.AllPossibleCards, Contains.Item(anyCard));
            }
        }

        #endregion

        #region Item Tests

        [Test]
        public async Task SaveItem_NoPopulation_ReturnedFromGet()
        {
            var cardNames = new string[]
            {
                "Ant Man",
                "Hawkeye",
                "Iron Man",
                "Rocket Raccoon",
                "Hulk",
                "Wolfsbane",
                "The Thing",
                "Gamora",
                "Klaw",
                "White Tiger",
                "Spectrum",
                "Heimdall"
            };

            var cards = cardNames.Select(n => SnapCards.ByName[n]).ToImmutableList();

            var item = new CardGeneSequence(
                cards,
                SnapCards.All,
                Guid.NewGuid(),
                200,
                new RandomCardOrder(),
                null,
                null,
                new MonteCarloSearchController(5)
            );

            await _repository.SaveItem(item);

            var savedItem = await _repository.GetItem<CardGeneSequence>(item.Id);

            Assert.NotNull(savedItem);
        }

        [Test]
        public async Task SaveItem_NoPopulation_ReturnsCardsInOrder()
        {
            var cardNames = new string[]
            {
                "Ant Man",
                "Hawkeye",
                "Iron Man",
                "Rocket Raccoon",
                "Hulk",
                "Wolfsbane",
                "The Thing",
                "Gamora",
                "Klaw",
                "White Tiger",
                "Spectrum",
                "Heimdall"
            };

            var cards = cardNames.Select(n => SnapCards.ByName[n]).ToImmutableList();

            var item = new CardGeneSequence(
                cards,
                SnapCards.All,
                Guid.NewGuid(),
                200,
                new RandomCardOrder(),
                null,
                null,
                new MonteCarloSearchController(5)
            );

            await _repository.SaveItem(item);

            var savedItem = await _repository.GetItem<CardGeneSequence>(item.Id);

            Assert.NotNull(savedItem);
            Assert.That(savedItem.Cards, Is.EquivalentTo(item.Cards));
        }

        [Test]
        public async Task SaveItem_WithPopulation_ReturnedFromGet()
        {
            var experimentId = await SaveExperiment("Cards In Order Experiment");
            var populationId = await SavePopulation(experimentId, "Cards In Order Pop");

            var cardNames = new string[]
            {
                "Ant Man",
                "Hawkeye",
                "Iron Man",
                "Rocket Raccoon",
                "Hulk",
                "Wolfsbane",
                "The Thing",
                "Gamora",
                "Klaw",
                "White Tiger",
                "Spectrum",
                "Heimdall"
            };

            var cards = cardNames.Select(n => SnapCards.ByName[n]).ToImmutableList();

            var item = new CardGeneSequence(
                cards,
                SnapCards.All,
                Guid.NewGuid(),
                200,
                new RandomCardOrder(),
                null,
                null,
                new MonteCarloSearchController(5)
            );

            await _repository.SaveItem(item);
            await _repository.AddItemToPopulation(item.Id, populationId, 0);

            var savedItem = await _repository.GetItem<CardGeneSequence>(item.Id);

            Assert.NotNull(savedItem);
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(5)]
        public async Task SaveItem_WithPopulation_ReturnedFromGetByPopulationAndGeneration(
            int generation
        )
        {
            var experimentId = await SaveExperiment("Cards In Order Experiment");
            var populationId = await SavePopulation(experimentId, "Cards In Order Pop");

            var cardNames = new string[]
            {
                "Ant Man",
                "Hawkeye",
                "Iron Man",
                "Rocket Raccoon",
                "Hulk",
                "Wolfsbane",
                "The Thing",
                "Gamora",
                "Klaw",
                "White Tiger",
                "Spectrum",
                "Heimdall"
            };

            var cards = cardNames.Select(n => SnapCards.ByName[n]).ToImmutableList();

            var item = new CardGeneSequence(
                cards,
                SnapCards.All,
                Guid.NewGuid(),
                200,
                new RandomCardOrder(),
                null,
                null,
                new MonteCarloSearchController(5)
            );

            await _repository.SaveItem(item);
            await _repository.AddItemToPopulation(item.Id, populationId, generation);

            var savedItems = await _repository.GetItems<CardGeneSequence>(populationId, generation);

            Assert.That(savedItems, Has.Exactly(1).Items);
            Assert.That(savedItems[0].Id, Is.EqualTo(item.Id));
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(5)]
        public async Task SaveItem_WithPopulation_ReturnsCardsInOrder(int generation)
        {
            var experimentId = await SaveExperiment("Cards In Order Experiment");
            var populationId = await SavePopulation(experimentId, "Cards In Order Pop");

            var cardNames = new string[]
            {
                "Ant Man",
                "Hawkeye",
                "Iron Man",
                "Rocket Raccoon",
                "Hulk",
                "Wolfsbane",
                "The Thing",
                "Gamora",
                "Klaw",
                "White Tiger",
                "Spectrum",
                "Heimdall"
            };

            var cards = cardNames.Select(n => SnapCards.ByName[n]).ToImmutableList();

            var item = new CardGeneSequence(
                cards,
                SnapCards.All,
                Guid.NewGuid(),
                200,
                new RandomCardOrder(),
                null,
                null,
                new MonteCarloSearchController(5)
            );

            await _repository.SaveItem(item);
            await _repository.AddItemToPopulation(item.Id, populationId, generation);

            var savedItem = await _repository.GetItem<CardGeneSequence>(item.Id);

            Assert.NotNull(savedItem);
            Assert.That(savedItem.Cards, Is.EquivalentTo(item.Cards));

            savedItem = (
                await _repository.GetItems<CardGeneSequence>(populationId, generation)
            ).SingleOrDefault();

            Assert.NotNull(savedItem);
            Assert.That(savedItem.Cards, Is.EquivalentTo(item.Cards));
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(5)]
        public async Task SaveItem_WithPopulation_ReturnsFixedCards(int generation)
        {
            var fixedCardNames = new[] { "Gamora", "Iron Man" };

            var experimentId = await SaveExperiment("Cards In Order Experiment");
            var populationId = await SavePopulation(
                experimentId,
                "Cards In Order Pop",
                fixedCardNames
            );

            var cardNames = new string[]
            {
                "Ant Man",
                "Hawkeye",
                "Rocket Raccoon",
                "Hulk",
                "Wolfsbane",
                "The Thing",
                "Klaw",
                "White Tiger",
                "Spectrum",
                "Heimdall"
            };

            var cards = cardNames.Select(n => SnapCards.ByName[n]).ToImmutableList();
            var fixedCards = fixedCardNames.Select(n => SnapCards.ByName[n]).ToImmutableList();

            var item = new PartiallyFixedCardGeneSequence(
                new FixedCardGeneSequence(fixedCards, Guid.Empty),
                new CardGeneSequence(
                    cards,
                    SnapCards.All,
                    Guid.Empty,
                    200,
                    new RandomCardOrder(),
                    null,
                    null
                ),
                new MonteCarloSearchController(5),
                Guid.NewGuid(),
                null,
                null
            );

            await _repository.SaveItem(item);
            await _repository.AddItemToPopulation(item.Id, populationId, generation);

            var savedItem = await _repository.GetItem<PartiallyFixedCardGeneSequence>(item.Id);

            Assert.NotNull(savedItem);
            Assert.That(savedItem.FixedCards.Cards, Is.EquivalentTo(item.FixedCards.Cards));
            Assert.That(savedItem.EvolvingCards.Cards, Is.EquivalentTo(item.EvolvingCards.Cards));

            savedItem = (
                await _repository.GetItems<PartiallyFixedCardGeneSequence>(populationId, generation)
            ).SingleOrDefault();

            Assert.NotNull(savedItem);
            Assert.That(savedItem.FixedCards.Cards, Is.EquivalentTo(item.FixedCards.Cards));
            Assert.That(savedItem.EvolvingCards.Cards, Is.EquivalentTo(item.EvolvingCards.Cards));
        }

        #endregion

        #region Helper Functions

        /// <summary>
        /// Gets the current time (i.e. <see cref="DateTimeOffset.UtcNow"/>) but truncated
        /// to the nearest microsecond, to account for the fact that Postgresql is one
        /// decimal point less precise, and it'll cause test failures otherwise.
        /// </summary>
        private DateTimeOffset TruncatedNow()
        {
            var now = DateTimeOffset.UtcNow;
            var extraTicks = now.Ticks % 10;
            return now.Subtract(TimeSpan.FromTicks(extraTicks));
        }

        /// <summary>
        /// Saves a test <see cref="Experiment"/> and returns the <see cref="Experiment.Id"/>;
        /// </summary>
        private async Task<Guid> SaveExperiment(string name)
        {
            var experiment = new Experiment(Guid.NewGuid(), name, DateTimeOffset.UtcNow);

            await _repository.SaveExperiment(experiment);

            return experiment.Id;
        }

        /// <summary>
        /// Saves a test <see cref="Population{T}"/> and returns the <see cref="Population{T}.Id"/>;
        ///
        /// If any fixed cards are given, it will be a <see cref="Population{PartiallyFixedCardGeneSequence}"/>.
        /// Otherwise, it will be a <see cref="Population{CardGeneSequence}"/>.
        /// </summary>
        private async Task<Guid> SavePopulation(
            Guid experimentId,
            string name,
            params string[] fixedCardNames
        )
        {
            if (fixedCardNames.Length == 0)
            {
                var population = new Population<CardGeneSequence>(
                    new CardGenetics(
                        SnapCards.All,
                        new MonteCarloSearchController(5),
                        200,
                        new RandomCardOrder(),
                        12
                    ),
                    new List<CardGeneSequence>(),
                    name,
                    Guid.NewGuid(),
                    experimentId,
                    0,
                    TruncatedNow()
                );

                await _repository.SavePopulation(population);

                return population.Id;
            }
            else
            {
                var fixedCards = fixedCardNames.Select(n => SnapCards.ByName[n]).ToImmutableList();

                var population = new Population<PartiallyFixedCardGeneSequence>(
                    new PartiallyFixedGenetics(
                        fixedCards,
                        SnapCards.All,
                        new MonteCarloSearchController(5),
                        200,
                        new RandomCardOrder()
                    ),
                    new List<PartiallyFixedCardGeneSequence>(),
                    name,
                    Guid.NewGuid(),
                    experimentId,
                    0,
                    TruncatedNow()
                );

                await _repository.SavePopulation(population);

                return population.Id;
            }
        }

        private async Task SaveAllCardDefinitions()
        {
            foreach (var card in SnapCards.All)
            {
                await _repository.SaveCardDefinition(card);
            }
        }

        private async Task ClearRepository()
        {
            using var datasource = NpgsqlDataSource.Create(_connectionString);
            using var connection = await datasource.OpenConnectionAsync();

            await connection.ExecuteAsync("DELETE FROM item_carddefinition");
            await connection.ExecuteAsync("DELETE FROM population_item");
            await connection.ExecuteAsync("DELETE FROM item");
            await connection.ExecuteAsync("DELETE FROM population_carddefinition");
            await connection.ExecuteAsync("DELETE FROM population");
            await connection.ExecuteAsync("DELETE FROM experiment");
            await connection.ExecuteAsync("DELETE FROM carddefinition");
        }

        #endregion
    }
}
