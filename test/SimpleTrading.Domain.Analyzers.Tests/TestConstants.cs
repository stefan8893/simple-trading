using SimpleTrading.Domain.Analyzers.Tests.TestSourceFiles;

namespace SimpleTrading.Domain.Analyzers.Tests;

public static class TestConstants
{
    public const string InteractorSource = """
                                           namespace SimpleTrading.Domain.Infrastructure;

                                           public interface IInteractor<TResponseModel>
                                           {
                                               Task<TResponseModel> Execute();
                                           }

                                           public interface IInteractor<in TRequestModel, TResponseModel>
                                           {
                                               Task<TResponseModel> Execute(TRequestModel model);
                                           }
                                           """;

    private static readonly string CurrentDirectory = Environment.CurrentDirectory;


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