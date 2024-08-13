using System.CommandLine;
using SimpleTrading.WebApi.Clients;

namespace SimpleTrading.WebApi.CliCommands;

public enum Target
{
    CSharp,
    TypeScript
}

public static class GenerateClientCommand
{
    private static readonly Option<Target> TargetOption = new(
            ["--target", "-t"],
            "The Language for which the client will be generated")
        {IsRequired = true};

    private static readonly Option<DirectoryInfo[]> OutputDirOption = new(
        ["--output-dir", "-o"],
        () => [new DirectoryInfo(".")],
        "The directory in which the client will be generated");

    private static readonly Option<string> FileNameOption = new(
            "--file-name",
            "The file name of the generated client. e.g. SimpleTrading.Client.cs")
        {IsRequired = true};

    public static Command Create(WebApplication app)
    {
        var generateClientCommand = new Command("generate-client", "Creates a client for the web api");

        generateClientCommand.AddOption(TargetOption);
        generateClientCommand.AddOption(OutputDirOption);
        generateClientCommand.AddOption(FileNameOption);

        generateClientCommand.SetHandler(async (target, fileName, outputDir) =>
            {
                var clientGenerator = app.Services.GetRequiredService<ClientGenerator>();
                await clientGenerator.Generate(target, outputDir, fileName);
            },
            TargetOption,
            FileNameOption,
            OutputDirOption);

        return generateClientCommand;
    }
}