using System.CommandLine;
using System.CommandLine.Invocation;
using SimpleTrading.Domain.DataAccess;
using SimpleTrading.WebApi.Clients;

namespace SimpleTrading.WebApi;

public static class CliCommands
{
    private const string SqlServerProviderName = "Microsoft.EntityFrameworkCore.SqlServer";

    public static RootCommand RootCommand(WebApplication app)
    {
        var rootCommand = new RootCommand("Starts the web api")
        {
            TreatUnmatchedTokensAsErrors = false
        };

        rootCommand.SetHandler(() => app.Run());
        return rootCommand;
    }

    public static Command CreateDatabaseCommand(WebApplication app)
    {
        var createDbCommand = new Command("create-db",
            "Creates the schema without data. Run seed-data afterwards to add master data");

        var dropExistingOption = new Option<bool>("--drop-existing",
            () => false,
            "An already existing database gets dropped");

        var forceOption = new Option<bool>(["-f", "--force"],
            "Drop the db even it's 'SqlServer'");

        createDbCommand.AddOption(dropExistingOption);
        createDbCommand.AddOption(forceOption);
        createDbCommand.SetHandler(CreateDatabase);

        return createDbCommand;

        async Task CreateDatabase(InvocationContext ctx)
        {
            using var scope = app.Services.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            var dbContext = scope.ServiceProvider.GetRequiredService<TradingDbContext>();
            var dropExisting = ctx.ParseResult.GetValueForOption(dropExistingOption);
            var forceDeletion = ctx.ParseResult.GetValueForOption(forceOption);
            var cancellationToken = ctx.GetCancellationToken();

            if (dropExisting && !forceDeletion && dbContext.Database.ProviderName == SqlServerProviderName)
            {
                logger.LogWarning("The existing database is '{dbProviderName}'", SqlServerProviderName);
                logger.LogWarning("If you really want to delete it add the '--force' switch");
                logger.LogWarning("But watch out not to delete the production database");

                return;
            }

            if (dropExisting)
                await DeleteExistingDb(dbContext, cancellationToken, logger);

            logger.LogInformation("New {dbProviderName} database will be created", dbContext.Database.ProviderName);
            await dbContext.Database.EnsureCreatedAsync(cancellationToken);
            logger.LogInformation("New {dbProviderName} database successfully created",
                dbContext.Database.ProviderName);
        }

        async Task DeleteExistingDb(TradingDbContext dbContext, CancellationToken cancellationToken,
            ILogger<Program> logger)
        {
            var wasDeleted = await dbContext.Database.EnsureDeletedAsync(cancellationToken);

            if (wasDeleted)
                logger.LogInformation("Database {dbProviderName} dropped", dbContext.Database.ProviderName);
            else
                logger.LogInformation("An existing database didn't exist");
        }
    }

    public static Command SeedDatabaseCommand(WebApplication app)
    {
        var seedDataCommand = new Command("seed-data",
            "Data seeding is the process of populating a database with an initial set of data");
        seedDataCommand.SetHandler(async () =>
        {
            using var scope = app.Services.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

            logger.LogInformation("Seed Data into database ...");

            await scope.ServiceProvider.GetRequiredService<DbMasterData>().Populate();

            logger.LogInformation("Database successfully populated with master data");
        });

        return seedDataCommand;
    }

    public static Command DropDatabaseCommand(WebApplication app)
    {
        var dropDbCommand = new Command("drop-db",
            "Drops the database, but only if it is not 'SqlServer'. Be careful, all your data will be lost");

        var forceOption = new Option<bool>(["-f", "--force"],
            "Drop the db even it's 'SqlServer'");

        dropDbCommand.AddOption(forceOption);
        dropDbCommand.SetHandler(async ctx =>
        {
            using var scope = app.Services.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            var dbContext = scope.ServiceProvider.GetRequiredService<TradingDbContext>();
            var forceDeletion = ctx.ParseResult.GetValueForOption(forceOption);
            var cancellationToken = ctx.GetCancellationToken();

            if (!forceDeletion && dbContext.Database.ProviderName == SqlServerProviderName)
            {
                logger.LogWarning("The existing database is '{dbProviderName}'", SqlServerProviderName);
                logger.LogWarning("If you really want to delete it add the '--force' switch");
                logger.LogWarning("But watch out not to delete the production database");

                return;
            }

            await dbContext.Database.EnsureDeletedAsync(cancellationToken);
            logger.LogInformation("Database {dbProviderName} dropped", dbContext.Database.ProviderName);
        });

        return dropDbCommand;
    }

    public static Command GenerateClientCommand(WebApplication app)
    {
        var generateClientCommand = new Command("generate-client", "Creates a client for the web api");

        var targetOption =
            new Option<Target>(
                    ["--target", "-t"],
                    "The Language for which the client will be generated")
                {IsRequired = true};

        var outputDirOption =
            new Option<DirectoryInfo[]>(
                ["--output-dir", "-o"],
                () => [new DirectoryInfo(".")],
                "The directory in which the client will be generated");

        var fileNameOption =
            new Option<string>(
                    "--file-name",
                    "The file name of the generated client. e.g. SimpleTrading.Client.cs")
                {IsRequired = true};

        generateClientCommand.AddOption(targetOption);
        generateClientCommand.AddOption(outputDirOption);
        generateClientCommand.AddOption(fileNameOption);

        generateClientCommand.SetHandler(async (target, fileName, outputDir) =>
            {
                var clientGenerator = app.Services.GetRequiredService<ClientGenerator>();
                await clientGenerator.Generate(target, outputDir, fileName);
            },
            targetOption,
            fileNameOption,
            outputDirOption);

        return generateClientCommand;
    }
}