using Dapper;
using Npgsql;
using Testcontainers.PostgreSql;

namespace Snapdragon.Postgresql.Tests
{
    public class PostgresqlSnapdragonRepositoryTests
    {
        private const int Port = 9037; // Just a random number

        private PostgreSqlContainer _container;
        private string _connectionString = string.Empty;

        private PostgresqlSnapdragonRepository _repository;

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
        }

        [Test]
        public void SaveExperiment_ReturnedFromGet()
        {
            var experiment = new GeneticAlgorithm.Experiment(
                Guid.NewGuid(),
                "Test Saved Experiment",
                TruncatedNow()
            );

            _repository.SaveExperiment(experiment);

            var savedExperiment = _repository.GetExperiment(experiment.Id);

            Assert.That(savedExperiment, Is.EqualTo(experiment));
        }

        [Test]
        public void SaveExperiment_ReturnedFromGetAll()
        {
            var experiment = new GeneticAlgorithm.Experiment(
                Guid.NewGuid(),
                "Test Saved Experiment In List",
                TruncatedNow()
            );

            _repository.SaveExperiment(experiment);

            var savedExperiments = _repository.GetExperiments();

            Assert.That(savedExperiments.Count, Is.EqualTo(1));
            Assert.That(savedExperiments[0], Is.EqualTo(experiment));
        }

        [Test]
        public void DeleteExperiment_NoLongerRetrievable()
        {
            var experiment = new GeneticAlgorithm.Experiment(
                Guid.NewGuid(),
                "Test Deleted Experiment",
                TruncatedNow()
            );

            _repository.SaveExperiment(experiment);
            _repository.DeleteExperiment(experiment.Id);

            var savedExperiment = _repository.GetExperiment(experiment.Id);
            Assert.That(savedExperiment, Is.Null);

            var savedExperiments = _repository.GetExperiments();
            Assert.That(savedExperiments, Is.Empty);
        }

        [Test]
        public void GetNonexistentExperiment_ReturnsNull()
        {
            var noExperiment = _repository.GetExperiment(Guid.Empty);

            Assert.That(noExperiment, Is.Null);
        }

        [Test]
        [TestCase("Test Card 1", 1, 3)]
        [TestCase("Test Card 2", 5, -1)]
        public void SaveCardDefinition_ReturnedFromGet(string name, int cost, int power)
        {
            var card = new CardDefinition(name, cost, power);
            _repository.SaveCardDefinition(card);

            var savedCard = _repository.GetCardDefinition(card.Name);

            Assert.That(savedCard, Is.EqualTo(card));
        }

        [Test]
        [TestCase("Test Card 3", 2, 2)]
        [TestCase("Test Card 4", 6, 12)]
        public void SaveCardDefinition_ReturnedFromGetAll(string name, int cost, int power)
        {
            var card = new CardDefinition(name, cost, power);
            _repository.SaveCardDefinition(card);

            var savedCards = _repository.GetCardDefinitions();

            Assert.That(savedCards.Count, Is.EqualTo(1));
            Assert.That(savedCards[0], Is.EqualTo(card));
        }

        [Test]
        [TestCase("Test Deleted Card 1", 1, 0)]
        [TestCase("Test Deleted Card 2", 0, 1)]
        public void DeleteCardDefinition_NoLongerRetrievable(string name, int cost, int power)
        {
            var card = new CardDefinition(name, cost, power);
            _repository.SaveCardDefinition(card);

            _repository.DeleteCardDefinition(card.Name);

            var savedCard = _repository.GetCardDefinition(card.Name);
            Assert.That(savedCard, Is.Null);

            var savedCards = _repository.GetCardDefinitions();
            Assert.That(savedCards.Count, Is.EqualTo(0));
        }

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

        private async Task ClearRepository()
        {
            using var datasource = NpgsqlDataSource.Create(_connectionString);
            using (var connection = datasource.OpenConnection())
            {
                await connection.ExecuteAsync("DELETE FROM experiment");
                await connection.ExecuteAsync("DELETE FROM carddefinition");
            }
        }
    }
}
