using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Lms.Infrastructure.Database;
public sealed class DatabaseConnection : IDisposable
{
    public const string ConnectionName = "lmsdb";

    private readonly NpgsqlDataSource _dataSource;

    public DatabaseConnection(string connectionString)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        _dataSource = NpgsqlDataSource.Create(connectionString);
    }

    /// <summary>
    /// Reads the connection string from appsettings.json, appsettings.{environment}.json and
    /// environment variables, in that order of precedence. Aspire and production supply it
    /// through the environment; for your own Postgres put it in appsettings.Development.json.
    /// </summary>
    public static DatabaseConnection FromConfiguration()
    {
        var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Development";

        var connectionString = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .AddEnvironmentVariables()
            .Build()
            .GetConnectionString(ConnectionName);

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                $"No '{ConnectionName}' connection string found. Run through the Aspire AppHost, " +
                $"add one to appsettings.{environment}.json next to the executable, " +
                $"or set the environment variable ConnectionStrings__{ConnectionName}.");
        }

        return new DatabaseConnection(connectionString);
    }

    public ValueTask<NpgsqlConnection> OpenAsync(CancellationToken cancellationToken = default)
    {
        return _dataSource.OpenConnectionAsync(cancellationToken);
    }

    public NpgsqlCommand CreateCommand(string sql)
    {
        return _dataSource.CreateCommand(sql);
    }

    public async Task<bool> CanConnectAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await using var command = CreateCommand("SELECT 1");
            await command.ExecuteScalarAsync(cancellationToken);
            return true;
        }
        catch (Exception ex) when (ex is NpgsqlException or TimeoutException)
        {
            return false;
        }
    }

    public void Dispose()
    {
        _dataSource.Dispose();
    }
}
