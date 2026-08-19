using DbUp;
using DbUp.Engine;
using Lms.Infrastructure.Database;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace Lms.Infrastructure.Migrations;

public static class Migrator
{


    // "0001_initial.sql".
    private const string ScriptNamespace = $"{nameof(Migrations)}.Scripts.";

    public static void MigrateDatabase()
    {
        MigrateDatabase(DatabaseConnection.BuildConfiguration());
    }

    public static void MigrateDatabase(IConfiguration configuration)
    {
        var connectionString = DatabaseConnection.GetConnectionString(configuration);

        EnsureDatabase.For.PostgresqlDatabase(connectionString);

        var upgrader = DeployChanges.To
            .PostgresqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(
                Assembly.GetExecutingAssembly(),
                name => name.StartsWith(ScriptNamespace, StringComparison.Ordinal)
                    && name.EndsWith(".sql", StringComparison.OrdinalIgnoreCase))
            .WithTransactionPerScript()
            .WithVariablesDisabled()
            .LogToConsole()
            .Build();

        EnsureScriptsWereFound(upgrader);

        var result = upgrader.PerformUpgrade();

        if (!result.Successful)
        {
            throw new InvalidOperationException(
                $"Database migration failed on script '{result.ErrorScript?.Name ?? "<unknown>"}'.",
                result.Error);
        }
    }
    private static void EnsureScriptsWereFound(UpgradeEngine upgrader)
    {
        if (upgrader.GetDiscoveredScripts().Count == 0)
        {
            throw new InvalidOperationException(
                $"No migration scripts were found under '{ScriptNamespace}'. Add your .sql files to " +
                "Migrations/Scripts and make sure they are included as <EmbeddedResource> in Lms.Infrastructure.csproj.");
        }
    }
}
