using SimpleTrading.Domain.Analyzers.Tests.TestCases;

namespace SimpleTrading.Domain.Analyzers.Tests;

public static class TestConstants
{
    private static readonly string CurrentDirectory = Environment.CurrentDirectory;

    public static readonly string PathToTestCasesProjectFile = Path.Combine(Environment.CurrentDirectory,
        "../../../../SimpleTrading.Domain.Analyzers.Tests.ProjectTemplate/SimpleTrading.Domain.Analyzers.Tests.ProjectTemplate.csproj");


    public static class TestCases
    {
        private static readonly string TestCasesDir = Path.Combine(CurrentDirectory, nameof(TestCases));

        public static readonly string ValidatorAndBadInputCaseExistsCase =
            Path.Combine(TestCasesDir, $"{nameof(ValidatorAndBadInputCaseExists)}.cs");

        public static readonly string ValidatorExistsForRequestModelCase =
            Path.Combine(TestCasesDir, $"{nameof(ValidatorExistsForRequestModel)}.cs");
        
        public static readonly string ResponseModelTypeIsNotOneOfCase = 
            Path.Combine(TestCasesDir, $"{nameof(ResponseModelTypeIsNotOneOf)}.cs");
    }
}