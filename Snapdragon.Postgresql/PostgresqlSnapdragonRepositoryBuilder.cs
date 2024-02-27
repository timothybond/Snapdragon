namespace Snapdragon.Postgresql
{
    public class PostgresqlSnapdragonRepositoryBuilder : ISnapdragonRepositoryBuilder
    {
        public PostgresqlSnapdragonRepositoryBuilder(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public string ConnectionString { get; }

        public ISnapdragonRepository Build()
        {
            return new PostgresqlSnapdragonRepository(ConnectionString);
        }
    }
}
