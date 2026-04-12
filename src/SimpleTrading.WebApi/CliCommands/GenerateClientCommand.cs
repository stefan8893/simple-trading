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
        "TargetLanguage", "--target", "-t")
    {
        Required = true,
        Description = "The Language for which the client will be generated"
    };

    private static readonly Option<DirectoryInfo[]> OutputDirOption = new("OutputDirectory", "--output-dir", "-o")
    {
        Description = "The directory in which the client will be generated",
        DefaultValueFactory = _ => [new DirectoryInfo(".")]
    };

    private static readonly Option<string> FileNameOption = new("Filename", "--file-name"
    )
    {
        Required = true,
        Description = "The file name of the generated client. e.g. SimpleTrading.Client.cs"
    };

    public static Command Create(WebApplication app)
    {
        var generateClientCommand = new Command("generate-client", "Creates a client for the web api");

        generateClientCommand.Options.Add(TargetOption);
        generateClientCommand.Options.Add(OutputDirOption);
        generateClientCommand.Options.Add(FileNameOption);

        generateClientCommand.SetAction(parseResult =>
        {
            var clientGenerator = app.Services.GetRequiredService<ClientGenerator>();
            var target = parseResult.GetValue(TargetOption);
            var outputDir = parseResult.GetValue(OutputDirOption)!;
            var fileName = parseResult.GetValue(FileNameOption)!;

            return clientGenerator.Generate(target, outputDir, fileName);
        });

        return generateClientCommand;
    }
}