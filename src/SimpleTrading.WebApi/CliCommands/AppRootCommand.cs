using System.CommandLine;

namespace SimpleTrading.WebApi.CliCommands;

public static class AppRootCommand
{
    public static RootCommand Create(WebApplication app)
    {
        var rootCommand = new RootCommand("Starts the web api")
        {
            TreatUnmatchedTokensAsErrors = false
        };

        rootCommand.SetHandler(() => app.Run());
        return rootCommand;
    }
}