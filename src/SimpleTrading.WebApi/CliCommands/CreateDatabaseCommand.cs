using System.CommandLine;
using System.CommandLine.Invocation;
using Microsoft.EntityFrameworkCore;
using SimpleTrading.DataAccess;

namespace SimpleTrading.WebApi.CliCommands;

public static class CreateDatabaseCommand
{
    private const string SqlServerProviderName = "Microsoft.EntityFrameworkCore.SqlServer";

    private static readonly Command CreateDbCommand = new("create-db",
        "Creates the database and the schema, but without data. Run seed-data afterwards to add master data");

    private static readonly Option<bool> DropExistingOption = new("--drop-existing",
        () => false,
        "An already existing database gets dropped");

    private static readonly Option<bool> ForceOption = new(["-f", "--force"],
        "Drop the db even it's SqlServer");

    public static Command Create(WebApplication app)
    {
        CreateDbCommand.AddOption(DropExistingOption);
        CreateDbCommand.AddOption(ForceOption);
        CreateDbCommand.SetHandler(ctx => CreateDatabase(ctx, app));

        return CreateDbCommand;
    }

    private static async Task CreateDatabase(InvocationContext ctx, IHost app)
    {
        using var scope = app.Services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        var dbContext = scope.ServiceProvider.GetRequiredService<TradingDbContext>();
        var dropExisting = ctx.ParseResult.GetValueForOption(DropExistingOption);
        var forceDeletion = ctx.ParseResult.GetValueForOption(ForceOption);
        var cancellationToken = ctx.GetCancellationToken();

        switch (dropExisting)
        {
            case true when !forceDeletion && dbContext.Database.ProviderName == SqlServerProviderName:
                logger.LogWarning("The existing database is '{dbProviderName}'", SqlServerProviderName);
                logger.LogWarning("If you really want to delete it add the '--force' switch");
                logger.LogWarning("But watch out not to delete the production database");

                return;
            case true:
                await DeleteExistingDb(dbContext, logger, cancellationToken);
                break;
        }

        logger.LogInformation("New {dbProviderName} database will be created", dbContext.Database.ProviderName);
        await dbContext.Database.MigrateAsync(cancellationToken);
        logger.LogInformation("New {dbProviderName} database successfully created",
            dbContext.Database.ProviderName);
    }

    private static async Task DeleteExistingDb(DbContext dbContext, ILogger<Program> logger,
        CancellationToken cancellationToken)
    {
        var wasDeleted = await dbContext.Database.EnsureDeletedAsync(cancellationToken);

        if (wasDeleted)
            logger.LogInformation("Database {dbProviderName} dropped", dbContext.Database.ProviderName);
        else
            logger.LogInformation("An existing database didn't exist");
    }
}