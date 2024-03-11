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
        public async Task SavePopulation_GetsBackSameId()
        {
            var experimentId = await SaveExperiment("Test Id Experiment");

            var population = new Population<CardGeneSequence>(
                new CardGenetics(
                    SnapCards.All,
                    new RandomPlayerController(),
                    100,
                    new ExistingCardOrder()
                ),
                0,
                "Test Pop Id",
                experimentId
            );

            await _repository.SavePopulation(population);

            var savedPopulation = await _repository.GetPopulation<CardGeneSequence>(population.Id);

            Assert.That(savedPopulation, Is.Not.Null);
            Assert.That(savedPopulation.Id, Is.EqualTo(population.Id));
        }

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

        [Test]
        [TestCase()]
        [TestCase("Ant Man", "Misty Knight")]
        [TestCase("Iron Man", "Blade", "Hulk")]
        public async Task SavePopulation_FixedCards_GetsBackSameId(params string[] cardNames)
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
            Assert.That(savedPopulation.Id, Is.EqualTo(population.Id));
        }

        [Test]
        [TestCase()]
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

        [Test]
        public async Task SavePopulation_RetrievedByExperiment()
        {
            var experimentId = await SaveExperiment("Test Population By Experiment");

            var population1 = new Population<CardGeneSequence>(
                new CardGenetics(
                    SnapCards.All,
                    new MonteCarloSearchController(10),
                    25,
                    new ExistingCardOrder()
                ),
                0,
                "Test Pop By Experiment 1",
                experimentId
            );
            var population2 = new Population<CardGeneSequence>(
                new CardGenetics(
                    SnapCards.All,
                    new MonteCarloSearchController(10),
                    25,
                    new ExistingCardOrder()
                ),
                0,
                "Test Pop By Experiment 2",
                experimentId
            );

            await _repository.SavePopulation(population1);
            await _repository.SavePopulation(population2);

            var savedPops = await _repository.GetPopulations<CardGeneSequence>(experimentId);

            Assert.That(savedPops, Has.Exactly(2).Items);

            var savedPopNames = savedPops.Select(p => p.Name);
            Assert.That(savedPopNames, Contains.Item("Test Pop By Experiment 1"));
            Assert.That(savedPopNames, Contains.Item("Test Pop By Experiment 2"));
        }

        [Test]
        public async Task GetCardCount_GetsExpectedCounts()
        {
            var experimentId = await SaveExperiment("Test Experiment For Counts");

            var populationId = await SavePopulation(experimentId, "Test Population For Counts");

            var kaZarCards = new string[]
            {
                "Ka-Zar",
                "Squirrel Girl",
                "Blue Marvel",
                "Ant Man",
                "Blade",
                "Elektra",
                "Rocket Raccoon",
                "Okoye",
                "America Chavez",
                "Agent 13",
                "Armor",
                "Misty Knight"
            };
            var kaZarDeckId1 = await SaveItem(kaZarCards);
            var kaZarDeckId2 = await SaveItem(kaZarCards);

            var moveCards = new string[]
            {
                "Vulture",
                "Human Torch",
                "Doctor Strange",
                "Multiple Man",
                "Kraven",
                "Iron Fist",
                "Cloak",
                "Heimdall",
                "Medusa",
                "Iron Man",
                "Hawkeye",
                "Nightcrawler"
            };
            var moveDeckId1 = await SaveItem(moveCards);
            var moveDeckId2 = await SaveItem(moveCards);

            await _repository.AddItemToPopulation(kaZarDeckId1, populationId, 0);
            await _repository.AddItemToPopulation(kaZarDeckId2, populationId, 0);
            await _repository.AddItemToPopulation(moveDeckId1, populationId, 1);
            await _repository.AddItemToPopulation(moveDeckId2, populationId, 1);
            await _repository.AddItemToPopulation(kaZarDeckId1, populationId, 2);
            await _repository.AddItemToPopulation(moveDeckId1, populationId, 2);

            // Population needs to have an accurate generation value
            var population = await _repository.GetPopulation<CardGeneSequence>(populationId);
            Assert.That(population, Is.Not.Null);

            population = population with { Generation = 2 };
            await _repository.SavePopulation(population);

            var allCardCounts = await _repository.GetCardCounts<CardGeneSequence>(populationId);

            Assert.That(allCardCounts, Is.Not.Null);

            foreach (var cardName in kaZarCards)
            {
                var cardCounts = allCardCounts.SingleOrDefault(cc =>
                    string.Equals(cc.Name, cardName)
                );
                Assert.That(cardCounts, Is.Not.Null);
                Assert.That(cardCounts.Counts.SequenceEqual(new[] { 2, 0, 1 }));
            }

            foreach (var cardName in moveCards)
            {
                var cardCounts = allCardCounts.SingleOrDefault(cc =>
                    string.Equals(cc.Name, cardName)
                );
                Assert.That(cardCounts, Is.Not.Null);
                Assert.That(cardCounts.Counts.SequenceEqual(new[] { 0, 2, 1 }));
            }

            foreach (
                var cardName in SnapCards
                    .All.Select(c => c.Name)
                    .Except(kaZarCards)
                    .Except(moveCards)
            )
            {
                var cardCounts = allCardCounts.SingleOrDefault(cc =>
                    string.Equals(cc.Name, cardName)
                );
                Assert.That(cardCounts, Is.Not.Null);
                Assert.That(cardCounts.Counts.SequenceEqual(new[] { 0, 0, 0 }));
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

        #region Game / Log Tests

        [Test]
        public async Task GetNonexistentGame_ReturnsNull()
        {
            var savedGame = await _repository.GetGame(Guid.NewGuid());
            Assert.That(savedGame, Is.Null);
        }

        [Test]
        public async Task SaveGame_CanRetrieve()
        {
            var topItem = await SaveItem();
            var bottomItem = await SaveItem();

            var game = new GameRecord(Guid.NewGuid(), topItem, bottomItem);

            await _repository.SaveGame(game);

            var savedGame = await _repository.GetGame(game.GameId);
            Assert.That(savedGame, Is.Not.Null);
        }

        [Test]
        public async Task SaveGame_SetsPlayerIds()
        {
            var topItem = await SaveItem();
            var bottomItem = await SaveItem();

            var game = new GameRecord(Guid.NewGuid(), topItem, bottomItem);

            await _repository.SaveGame(game);

            var savedGame = await _repository.GetGame(game.GameId);

            Assert.That(savedGame, Is.Not.Null);

            Assert.That(savedGame.TopPlayerId, Is.EqualTo(game.TopPlayerId));
            Assert.That(savedGame.BottomPlayerId, Is.EqualTo(game.BottomPlayerId));
        }

        [Test]
        [TestCase(Side.Top)]
        [TestCase(Side.Bottom)]
        [TestCase(null)]
        public async Task SaveGame_SetsWinner(Side? side)
        {
            var topItem = await SaveItem();
            var bottomItem = await SaveItem();

            var game = new GameRecord(Guid.NewGuid(), topItem, bottomItem, side);

            await _repository.SaveGame(game);

            var savedGame = await _repository.GetGame(game.GameId);

            Assert.That(savedGame, Is.Not.Null);

            Assert.That(savedGame.Winner, Is.EqualTo(game.Winner));
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(5)]
        [TestCase(20)]
        public async Task SaveGame_SetsExperimentAndGeneration(int generation)
        {
            // Note: Testing these two things together because they aren't meaningful separately
            var experimentId = await SaveExperiment("Game Record Experiment");

            var topItem = await SaveItem();
            var bottomItem = await SaveItem();

            var game = new GameRecord(
                Guid.NewGuid(),
                topItem,
                bottomItem,
                null,
                experimentId,
                generation
            );

            await _repository.SaveGame(game);

            var savedGame = await _repository.GetGame(game.GameId);

            Assert.That(savedGame, Is.Not.Null);

            Assert.That(savedGame.ExperimentId, Is.EqualTo(game.ExperimentId));
            Assert.That(savedGame.Generation, Is.EqualTo(game.Generation));
        }

        [Test]
        public async Task DeleteGame_CannotRetrieve()
        {
            var topItem = await SaveItem();
            var bottomItem = await SaveItem();

            var game = new GameRecord(Guid.NewGuid(), topItem, bottomItem);
            await _repository.SaveGame(game);

            await _repository.DeleteGame(game.GameId);

            var savedGame = await _repository.GetGame(game.GameId);
            Assert.That(savedGame, Is.Null);
        }

        [Test]
        public async Task NoGameLogs_RetrievesEmptyList()
        {
            var topItem = await SaveItem();
            var bottomItem = await SaveItem();

            var game = new GameRecord(Guid.NewGuid(), topItem, bottomItem);
            await _repository.SaveGame(game);

            var logs = await _repository.GetGameLogs(game.GameId);

            Assert.That(logs, Is.Empty);
        }

        [Test]
        public async Task SaveGameLog_CanRetrieve()
        {
            var topItem = await SaveItem();
            var bottomItem = await SaveItem();

            var game = new GameRecord(Guid.NewGuid(), topItem, bottomItem);
            await _repository.SaveGame(game);

            var firstLog = new GameLogRecord(game.GameId, 0, 1, "Stuff");
            var secondLog = new GameLogRecord(game.GameId, 1, 1, "Other Stuff");

            await _repository.SaveGameLog(firstLog);
            await _repository.SaveGameLog(secondLog);

            var logs = await _repository.GetGameLogs(game.GameId);

            Assert.That(logs, Has.Exactly(2).Items);
            Assert.That(logs[0], Is.EqualTo(firstLog));
            Assert.That(logs[1], Is.EqualTo(secondLog));
        }

        [Test]
        public async Task SaveGameLogs_RetrievedInSpecifiedOrder()
        {
            var topItem = await SaveItem();
            var bottomItem = await SaveItem();

            var game = new GameRecord(Guid.NewGuid(), topItem, bottomItem);
            await _repository.SaveGame(game);

            var firstLog = new GameLogRecord(game.GameId, 0, 1, "Stuff");
            var secondLog = new GameLogRecord(game.GameId, 1, 1, "Other Stuff");

            await _repository.SaveGameLog(secondLog);
            await _repository.SaveGameLog(firstLog);

            var logs = await _repository.GetGameLogs(game.GameId);

            Assert.That(logs, Has.Exactly(2).Items);
            Assert.That(logs[0], Is.EqualTo(firstLog));
            Assert.That(logs[1], Is.EqualTo(secondLog));
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
        /// Saves a test <see cref="CardGeneSequence"/> (with no associated population)
        /// and returns the <see cref="CardGeneSequence.Id"/>;
        /// </summary>
        private async Task<Guid> SaveItem(params string[] cardNames)
        {
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

            return item.Id;
        }

        /// <summary>
        /// Saves a test <see cref="CardGeneSequence"/> (with no associated population)
        /// and returns the <see cref="CardGeneSequence.Id"/>;
        /// </summary>
        private async Task<Guid> SaveItem()
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

            return await SaveItem(cardNames);
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

            await connection.ExecuteAsync("DELETE FROM game_log");
            await connection.ExecuteAsync("DELETE FROM game");
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
