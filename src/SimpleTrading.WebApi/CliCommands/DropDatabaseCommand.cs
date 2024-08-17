using System.CommandLine;
using System.CommandLine.Invocation;
using SimpleTrading.DataAccess;
using SimpleTrading.Domain.DataAccess;

namespace SimpleTrading.WebApi.CliCommands;

public static class DropDatabaseCommand
{
    private const string SqlServerProviderName = "Microsoft.EntityFrameworkCore.SqlServer";

    private static readonly Command DropDbCommand = new("drop-db",
        "Drops the database, but only if it is not 'SqlServer'. Be careful, all your data will be lost");

    private static readonly Option<bool> ForceOption = new(["-f", "--force"],
        "Drop the db even it's 'SqlServer'");


    public static Command Create(WebApplication app)
    {
        DropDbCommand.AddOption(ForceOption);
        DropDbCommand.SetHandler(ctx => DropDatabase(ctx, app));

        return DropDbCommand;
    }

    private static async Task DropDatabase(InvocationContext ctx, IHost app)
    {
        using var scope = app.Services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        var dbContext = scope.ServiceProvider.GetRequiredService<TradingDbContext>();
        var forceDeletion = ctx.ParseResult.GetValueForOption(ForceOption);
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
    }
}