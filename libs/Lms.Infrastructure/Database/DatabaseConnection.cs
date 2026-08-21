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


    public static string Environment => System.Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Development";

    public static IConfiguration BuildConfiguration()
    {
        return new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{Environment}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();
    }

    
    public static string GetConnectionString(IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        var connectionString = configuration.GetConnectionString(ConnectionName);

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                $"No '{ConnectionName}' connection string found. Run through the Aspire AppHost, " +
                $"add one to appsettings.{Environment}.json next to the executable, " +
                $"or set the environment variable ConnectionStrings__{ConnectionName}.");
        }

        return connectionString;
    }

    public static DatabaseConnection FromConfiguration()
    {
        return new DatabaseConnection(GetConnectionString(BuildConfiguration()));
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
