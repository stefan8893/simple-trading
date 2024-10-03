using SimpleTrading.Domain.Analyzers.Tests.TestSourceFiles;

namespace SimpleTrading.Domain.Analyzers.Tests;

public static class TestConstants
{
    private static readonly string CurrentDirectory = Environment.CurrentDirectory;

    public static readonly string PathToTestProjectFile = Path.Combine(Environment.CurrentDirectory,
        "../../../../SimpleTrading.Domain.Analyzers.Tests.ProjectTemplate/SimpleTrading.Domain.Analyzers.Tests.ProjectTemplate.csproj");


    public static class TestSourceFiles
    {
        private static readonly string TestSourceFilesDir = Path.Combine(CurrentDirectory, nameof(TestSourceFiles));

        public static readonly string ValidatorAndBadInputCaseExistsFile =
            Path.Combine(TestSourceFilesDir, $"{nameof(ValidatorAndBadInputCaseExists)}.cs");

        public static readonly string ValidatorExistsForRequestModelFile =
            Path.Combine(TestSourceFilesDir, $"{nameof(ValidatorExistsForRequestModel)}.cs");

        public static readonly string ResponseModelTypeIsNotOneOfFile =
            Path.Combine(TestSourceFilesDir, $"{nameof(ResponseModelTypeIsNotOneOf)}.cs");
        
        public static readonly string InteractorWithMissingInteractorSuffixFile =
            Path.Combine(TestSourceFilesDir, $"{nameof(InteractorWithMissingInteractorSuffix)}.cs");
    }
}