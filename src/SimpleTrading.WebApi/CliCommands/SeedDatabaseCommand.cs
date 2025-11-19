using System.CommandLine;
using SimpleTrading.DataAccess;

namespace SimpleTrading.WebApi.CliCommands;

public static class SeedDatabaseCommand
{
    private static readonly Command SeedDataCommand = new("seed-data",
        "Data seeding is the process of populating a database with an initial set of data");

    public static Command Create(WebApplication app)
    {
        SeedDataCommand.SetAction(_ => SeedData(app));

        return SeedDataCommand;
    }

    private static async Task SeedData(WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        var dbMasterData = scope.ServiceProvider.GetRequiredService<DbMasterData>();

        logger.LogInformation("Seed Data into database ...");

        await dbMasterData.Seed();

        logger.LogInformation("Database successfully populated with master data");
    }
}