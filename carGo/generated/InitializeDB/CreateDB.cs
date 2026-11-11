using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace InitializeDB;

public static class CreateDB
{
    public static string Create()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production"}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var csMaster = config.GetConnectionString("DbConnectionMaster")
            ?? Environment.GetEnvironmentVariable("ConnectionStrings__DbConnectionMaster")
            ?? Environment.GetEnvironmentVariable("DB_CONNECTION_MASTER")
            ?? throw new InvalidOperationException("No master connection string provided.");

        var cs = config.GetConnectionString("DbConnection")
            ?? Environment.GetEnvironmentVariable("ConnectionStrings__DbConnection")
            ?? Environment.GetEnvironmentVariable("DB_CONNECTION")
            ?? throw new InvalidOperationException("No application connection string provided.");

        var databaseArg = config["Database:Name"] ?? throw new InvalidOperationException("No database name provided.");
        var integratedSecurity = bool.TryParse(config["Database:IntegratedSecurity"], out var parsedIntegratedSecurity) && parsedIntegratedSecurity;
        var database = EscapeSqlName(databaseArg);
        var databaseLiteral = EscapeSqlLiteral(databaseArg);

        var deleteDatabase = $"IF EXISTS(SELECT * FROM sys.databases WHERE name = '{databaseLiteral}') DROP DATABASE [{database}]";
        var createDatabase = $"CREATE DATABASE [{database}]";

        using var cnn = new SqlConnection(csMaster);
        cnn.Open();

        Execute(cnn, deleteDatabase);
        Execute(cnn, createDatabase);
        if (!integratedSecurity)
        {
            var userArg = config["Database:User"] ?? throw new InvalidOperationException("No database user provided.");
            var passArg = config["Database:Password"] ?? throw new InvalidOperationException("No database password provided.");
            var user = EscapeSqlName(userArg);
            var userLiteral = EscapeSqlLiteral(userArg);
            var pass = EscapeSqlLiteral(passArg);

            var createUser =
                $"IF NOT EXISTS(SELECT name FROM master.dbo.syslogins WHERE name = '{userLiteral}')" + Environment.NewLine +
                "BEGIN" + Environment.NewLine +
                $"    CREATE LOGIN [{user}] WITH PASSWORD=N'{pass}', DEFAULT_DATABASE=[master], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF" + Environment.NewLine +
                "END";
            var associatedUser = $"USE [{database}]; CREATE USER [{user}] FOR LOGIN [{user}]; USE [{database}]; EXEC sp_addrolemember N'db_owner', N'{userLiteral}'";

            Execute(cnn, createUser);
            Execute(cnn, associatedUser);
        }

        Console.WriteLine("Database created successfully.");
        return cs;
    }

    private static void Execute(SqlConnection connection, string sql)
    {
        using var cmd = new SqlCommand(sql, connection);
        cmd.ExecuteNonQuery();
    }

    private static string EscapeSqlName(string value) => value.Replace("]", "]]");

    private static string EscapeSqlLiteral(string value) => value.Replace("'", "''");
}