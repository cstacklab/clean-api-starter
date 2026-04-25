namespace CleanApiStarter.Infrastructure.Database;

public interface IDatabaseConnectionFactory
{
    IDbConnection CreateConnection();
}

public class PostgresConnectionFactory(string connectionString) : IDatabaseConnectionFactory
{
    public IDbConnection CreateConnection()
    {
        return new NpgsqlConnection(connectionString);
    }
}