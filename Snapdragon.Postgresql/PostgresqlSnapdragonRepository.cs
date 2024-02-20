using Dapper;
using Npgsql;
using Snapdragon.GeneticAlgorithm;

namespace Snapdragon.Postgresql
{
    public class PostgresqlSnapdragonRepository : ISnapdragonRepository, IDisposable
    {
        private readonly string _connectionString;
        private readonly NpgsqlDataSource _dataSource;
        private NpgsqlConnection? _connection = null;
        private bool disposed = false;

        public PostgresqlSnapdragonRepository(string connectionString)
        {
            _connectionString = connectionString;
            _dataSource = NpgsqlDataSource.Create(connectionString);
        }

        public void DeleteExperiment(Guid id)
        {
            var conn = GetConnection();

            conn.Execute("DELETE FROM experiment WHERE id = @Id", new { Id = id });
        }

        public void DeletePopulation(Guid id)
        {
            throw new NotImplementedException();
        }

        public Experiment? GetExperiment(Guid id)
        {
            var conn = GetConnection();

            var row = conn.QueryFirstOrDefault<Data.Experiment>(
                "SELECT Id, Name, Created FROM experiment WHERE Id = @Id",
                new { Id = id }
            );

            if (row == null)
            {
                return null;
            }

            return new Experiment(row.Id, row.Name, row.Created);
        }

        public IReadOnlyList<Experiment> GetExperiments()
        {
            var conn = GetConnection();

            var rows = conn.Query<Data.Experiment>("SELECT Id, Name, Created FROM experiment");

            return rows.Select(r => new Experiment(r.Id, r.Name, r.Created)).ToList();
        }

        public IGeneSequence<T>? GetItem<T>(Guid id)
            where T : IGeneSequence<T>
        {
            throw new NotImplementedException();
        }

        public IReadOnlyList<IGeneSequence<T>> GetItems<T>(Guid populationId)
            where T : IGeneSequence<T>
        {
            throw new NotImplementedException();
        }

        public IReadOnlyList<IGeneSequence<T>> GetItems<T>(Guid experimentId, int generation)
            where T : IGeneSequence<T>
        {
            throw new NotImplementedException();
        }

        public Population<T>? GetPopulation<T>(Guid id)
            where T : IGeneSequence<T>
        {
            throw new NotImplementedException();
        }

        public Population<T>? GetPopulation<T>(Guid experimentId, int generation)
            where T : IGeneSequence<T>
        {
            throw new NotImplementedException();
        }

        public void SaveExperiment(Experiment experiment)
        {
            var experimentRow = new Data.Experiment
            {
                Id = experiment.Id,
                Name = experiment.Name,
                Created = experiment.Started
            };

            var conn = GetConnection();

            conn.Execute(
                "INSERT INTO experiment (Id, Name, Created) VALUES (@Id, @Name, @Created)",
                experimentRow
            );
        }

        public void SavePopulation<T>(Population<T> population)
            where T : IGeneSequence<T>
        {
            throw new NotImplementedException();
        }

        private NpgsqlConnection GetConnection()
        {
            // TODO: Handle thread safety if it ever ends up mattering
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(PostgresqlSnapdragonRepository));
            }

            _connection ??= _dataSource.OpenConnection();

            return _connection;
        }

        public void SaveCardDefinition(CardDefinition cardDefinition)
        {
            var row = (Data.CardDefinition)cardDefinition;

            var conn = GetConnection();

            conn.Execute(
                "INSERT INTO carddefinition (name, cost, power) VALUES (@Name, @Cost, @Power)",
                row
            );
        }

        public CardDefinition? GetCardDefinition(string cardName)
        {
            var conn = GetConnection();

            var row = conn.QueryFirstOrDefault<Data.CardDefinition>(
                "SELECT name, cost, power FROM carddefinition WHERE name = @Name",
                new { Name = cardName }
            );

            if (row == null)
            {
                return null;
            }

            return (Snapdragon.CardDefinition)row;
        }

        public IReadOnlyList<CardDefinition> GetCardDefinitions()
        {
            var conn = GetConnection();

            var rows = conn.Query<Data.CardDefinition>(
                "SELECT name, cost, power FROM carddefinition"
            );

            return rows.Select(cd => (CardDefinition)cd).ToList();
        }

        public void DeleteCardDefinition(string cardName)
        {
            var conn = GetConnection();

            conn.Execute("DELETE FROM carddefinition WHERE name = @Name", new { Name = cardName });
        }

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !disposed)
            {
                if (_connection != null)
                {
                    _connection.Dispose();
                    _connection = null;
                }

                _dataSource.Dispose();

                disposed = true;
            }
        }

        #endregion
    }
}
