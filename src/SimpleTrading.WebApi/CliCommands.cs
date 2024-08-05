using System.CommandLine;
using SimpleTrading.Domain.DataAccess;
using SimpleTrading.WebApi.Clients;

namespace SimpleTrading.WebApi;

public static class CliCommands
{
    public static RootCommand CreateRootCommand(WebApplication app)
    {
        var rootCommand = new RootCommand("Starts the web api")
        {
            TreatUnmatchedTokensAsErrors = false
        };

        rootCommand.SetHandler(() => app.Run());
        return rootCommand;
    }

    public static Command CreateSeedDataCommand(WebApplication app)
    {
        var seedDataCommand = new Command("seed-data",
            "Data seeding is the process of populating a database with an initial set of data");
        seedDataCommand.SetHandler(async () =>
        {
            using var scope = app.Services.CreateScope();
            await scope.ServiceProvider.GetRequiredService<DbMasterData>().Seed();
        });

        return seedDataCommand;
    }

    public static Command CreateGenerateClientCommand(WebApplication app)
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