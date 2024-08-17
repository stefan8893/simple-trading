using System.CommandLine;
using SimpleTrading.DataAccess;

namespace SimpleTrading.WebApi.CliCommands;

public static class SeedDatabaseCommand
{
    private static readonly Command SeedDataCommand = new("seed-data",
        "Data seeding is the process of populating a database with an initial set of data");

    public static Command Create(WebApplication app)
    {
        SeedDataCommand.SetHandler(() => SeedData(app));

        return SeedDataCommand;
    }

    private static async Task SeedData(IHost app)
    {
        using var scope = app.Services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        logger.LogInformation("Seed Data into database ...");

        await scope.ServiceProvider.GetRequiredService<DbMasterData>().Populate();

        logger.LogInformation("Database successfully populated with master data");
    }
}