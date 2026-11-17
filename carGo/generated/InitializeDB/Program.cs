using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using CarGo.Infrastructure.Logging;
using CarGo.Infrastructure.Repositories;

namespace InitializeDB;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        Console.WriteLine("-----------------------------------------------------------------------------");
        Console.WriteLine("A database called: CarGoNHibernate will be created if it does not exist.");
        Console.WriteLine("-----------------------------------------------------------------------------");

        try
        {
            var configuration = BuildConfiguration();
            using var loggerFactory = CreateLoggerFactory(configuration);
            var cfg = new Configuration();

            /*PROTECTED REGION ID(initializeDatabase) START*/
            bool freshDb;
            // === CarGo: SQLite protegido (todo en region protegida: sobrevive a regenerar) ===
            // La ruta de la BD sale de appsettings.json (ConnectionStrings:DbConnection), igual que hace HARIA Web.
            // Una ruta relativa se resuelve desde la carpeta del proyecto InitializeDB (como hace HARIA Web al
            // construir su propia cadena): "Data Source=CarGo.db" => generated/InitializeDB/CarGo.db.
            // NO depende del directorio de trabajo ni de bin/Debug.
            var projectDirInfo = new DirectoryInfo(AppContext.BaseDirectory);
            while (projectDirInfo is not null && !File.Exists(Path.Combine(projectDirInfo.FullName, "InitializeDB.csproj"))) projectDirInfo = projectDirInfo.Parent;
            var projectDir = (projectDirInfo ?? new DirectoryInfo(AppContext.BaseDirectory)).FullName;
            var rawCs = configuration["ConnectionStrings:DbConnection"] ?? string.Empty;
            var dataSource = rawCs.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(part => part.Split('=', 2))
                .Where(kv => kv.Length == 2 && kv[0].Trim().Equals("Data Source", StringComparison.OrdinalIgnoreCase))
                .Select(kv => kv[1].Trim())
                .FirstOrDefault();

            string dbPath;
            if (string.IsNullOrWhiteSpace(dataSource) || rawCs.Contains("Server=", StringComparison.OrdinalIgnoreCase))
            {
                // appsettings.json regenerado con SQL Server: caer a InitializeDB/CarGo.db
                dbPath = Path.Combine(projectDir, "CarGo.db");
            }
            else
            {
                dbPath = Path.GetFullPath(Path.IsPathRooted(dataSource) ? dataSource : Path.Combine(projectDir, dataSource));
            }
            var cs = $"Data Source={dbPath}";

            cfg.Configure(Path.Combine(AppContext.BaseDirectory, "hibernate.cfg.xml"));
            // Dialecto, driver y cadena se fijan por codigo: si HARIA regenera hibernate.cfg.xml con SQL Server,
            // esto sigue funcionando con SQLite.
            cfg.SetProperty(NHibernate.Cfg.Environment.Dialect, "NHibernate.Dialect.SQLiteDialect");
            cfg.SetProperty(NHibernate.Cfg.Environment.ConnectionDriver, "NHibernate.Driver.SQLite20Driver");
            cfg.SetProperty(NHibernate.Cfg.Environment.ConnectionString, cs);
            cfg.AddAssembly(typeof(UsuarioRepository).Assembly);
            cfg.EnableSqlStatementCapture();

            // SchemaExport(drop+create) BORRA TODO. Solo se ejecuta si la BD no existe o si pasas --reset
            // (dotnet run -- --reset). Antes de un --reset se guarda copia en <raiz del proyecto>/database/backups.
            var reset = args.Contains("--reset");
            freshDb = reset || !File.Exists(dbPath) || new FileInfo(dbPath).Length == 0;
            if (freshDb)
            {
                if (File.Exists(dbPath) && new FileInfo(dbPath).Length > 0)
                {
                    var root = new DirectoryInfo(Path.GetDirectoryName(dbPath)!);
                    while (root is not null && root.GetFiles("*.tdm").Length == 0) root = root.Parent;
                    var backupDir = Path.Combine(root?.FullName ?? Path.GetDirectoryName(dbPath)!, "database", "backups");
                    Directory.CreateDirectory(backupDir);
                    var backupFile = Path.Combine(backupDir, $"CarGo_{DateTime.Now:yyyyMMdd_HHmmss}.db");
                    File.Copy(dbPath, backupFile);
                    Console.WriteLine($"Backup guardado en: {backupFile}");
                }
                new SchemaExport(cfg).Execute(true, true, false);
                Console.WriteLine("-------------------------------------");
                Console.WriteLine($"Database schema created successfully: {dbPath}");
                Console.WriteLine("-------------------------------------");
            }
            else
            {
                Console.WriteLine($"La BD ya existe ({dbPath}). Se conservan los datos. Usa --reset para recrearla.");
            }
            /*PROTECTED REGION END*/

            Console.WriteLine("-------------------------------------------------------");
            Console.WriteLine("Let's initialize the data of your database!");
            Console.WriteLine("-------------------------------------------------------");

            /*PROTECTED REGION ID(seedData) START*/
            // Solo se siembra en una BD recien creada.
            // Se OMITE SeedData.InitializeDataAsync (autoprueba generada): crea datos de ejemplo y luego los borra, y con
            // el modelo actual falla al borrar el Viaje porque RemovePasajeroAsync (generado) nunca elimina la fila de
            // REL_UsuarioPasajero => "FOREIGN KEY constraint failed" y deja datos basura. No hace falta para la web.
            if (freshDb)
            {
                await SeedData.InitializeDataManualAsync(cfg, loggerFactory);
            }
            /*PROTECTED REGION END*/
            Console.WriteLine("-----------------------------------------");
            Console.WriteLine("The data has been inserted successfully!!");
            Console.WriteLine("-----------------------------------------");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private static IConfiguration BuildConfiguration() =>
        new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{System.Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production"}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

    private static ILoggerFactory CreateLoggerFactory(IConfiguration configuration)
    {
        var minimumLevel = Enum.TryParse<LogLevel>(configuration["Logging:File:MinimumLevel"], ignoreCase: true, out var parsedLevel)
            ? parsedLevel
            : LogLevel.Information;

        var configuredPath = configuration["Logging:File:Path"];
        var filePath = string.IsNullOrWhiteSpace(configuredPath)
            ? Path.Combine(GetLogBaseDirectory(), "logs", "application.log")
            : ResolveLogPath(configuredPath);

        return LoggerFactory.Create(builder =>
        {
            builder.SetMinimumLevel(minimumLevel);
            builder.AddProvider(new FileLoggerProvider(new FileLoggerOptions
            {
                FilePath = filePath,
                MinimumLevel = minimumLevel
            }));
        });
    }

    private static string ResolveLogPath(string configuredPath) =>
        Path.IsPathRooted(configuredPath)
            ? configuredPath
            : Path.Combine(GetLogBaseDirectory(), configuredPath);

    private static string GetLogBaseDirectory()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "InitializeDB.csproj")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        return AppContext.BaseDirectory;
    }
}