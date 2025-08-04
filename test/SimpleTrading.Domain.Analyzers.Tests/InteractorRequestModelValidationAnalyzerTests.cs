using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using OneOf;

namespace SimpleTrading.Domain.Analyzers.Tests;

[SuppressMessage("Usage", "xUnit1051:Calls to methods which accept CancellationToken should use TestContext.Current.CancellationToken")]
public class InteractorRequestModelValidationAnalyzerTests
{
    private readonly Project _testProject = CreateProject("SimpleTrading.Domain.Analyzer.Tests");

    private Project AddDocumentsToTestProject(Dictionary<string, string> files)
    {
        return files
            .Aggregate(_testProject,
                (current, file)
                    => current.AddDocument(file.Key, file.Value).Project);
    }

    [Fact]
    public async Task An_interactor_that_has_a_request_model_with_a_validator_must_have_a_bad_input_case_in_its_response_model()
    {
        // arrange
        var files = new Dictionary<string, string>
        {
            ["IInteractor.cs"] = TestConstants.InteractorSource,
            ["Test.cs"] = await File.ReadAllTextAsync(TestConstants.TestSourceFiles.ValidatorExistsForRequestModelFile, TestContext.Current.CancellationToken)
        };

        var project = AddDocumentsToTestProject(files);
        var compilation = await project.GetCompilationAsync(TestContext.Current.CancellationToken);
        Assert.NotNull(compilation);

        // act
        var diagnostics = await compilation
            .WithAnalyzers([new InteractorRequestModelValidationAnalyzer()]).GetAnalyzerDiagnosticsAsync(TestContext.Current.CancellationToken);

        // assert
        var missingBadInputCaseError = Assert.Single(diagnostics);
        Assert.Equal("ST0001", missingBadInputCaseError.Id);
        Assert.Equal("Test.cs", missingBadInputCaseError.Location.GetMappedLineSpan().Path);
        Assert.Equal("Usage", missingBadInputCaseError.Descriptor.Category);
        Assert.Equal(
            "Response model type 'OneOf' does not contain a case for 'BadInput', but this is required since there is a validator for 'GetFoobarRequestModel'",
            missingBadInputCaseError.GetMessage());
    }

    [Fact]
    public async Task An_interactor_that_has_a_request_model_with_a_validator_and_a_response_model_that_is_not_of_type_OneOf_results_in_an_error()
    {
        // arrange
        var files = new Dictionary<string, string>
        {
            ["IInteractor.cs"] = TestConstants.InteractorSource,
            ["Test.cs"] = await File.ReadAllTextAsync(TestConstants.TestSourceFiles.ResponseModelTypeIsNotOneOfFile, TestContext.Current.CancellationToken)
        };

        var project = AddDocumentsToTestProject(files);
        var compilation = await project.GetCompilationAsync(TestContext.Current.CancellationToken);
        Assert.NotNull(compilation);

        // act
        var diagnostics = await compilation
            .WithAnalyzers([new InteractorRequestModelValidationAnalyzer()]).GetAnalyzerDiagnosticsAsync(TestContext.Current.CancellationToken);

        // assert
        var responseTypeMustBeOneOfError = Assert.Single(diagnostics);
        Assert.Equal("ST0002", responseTypeMustBeOneOfError.Id);
        Assert.Equal("Test.cs", responseTypeMustBeOneOfError.Location.GetMappedLineSpan().Path);
        Assert.Equal("Usage", responseTypeMustBeOneOfError.Descriptor.Category);
        Assert.Equal(
            "Response model must be of type 'OneOf' including a 'BadInput' case, because there is a validator for 'GetFoobarRequestModel'",
            responseTypeMustBeOneOfError.GetMessage());
    }

    [Fact]
    public async Task An_interactor_that_has_a_request_model_with_a_validator_and_a_response_model_with_a_bad_input_case_does_not_result_in_an_error()
    {
        // arrange
        var files = new Dictionary<string, string>
        {
            ["IInteractor.cs"] = TestConstants.InteractorSource,
            ["Test.cs"] = await File.ReadAllTextAsync(TestConstants.TestSourceFiles.ValidatorAndBadInputCaseExistsFile, TestContext.Current.CancellationToken)
        };

        var project = AddDocumentsToTestProject(files);
        var compilation = await project.GetCompilationAsync(TestContext.Current.CancellationToken);
        Assert.NotNull(compilation);

        // act
        var diagnostics = await compilation
            .WithAnalyzers([new InteractorRequestModelValidationAnalyzer()]).GetAnalyzerDiagnosticsAsync(TestContext.Current.CancellationToken);

        // assert
        Assert.Empty(diagnostics);
    }

    [Fact]
    public async Task Interactor_names_must_end_with_Interactor()
    {
        // arrange
        var files = new Dictionary<string, string>
        {
            ["IInteractor.cs"] = TestConstants.InteractorSource,
            ["Test.cs"] = await File.ReadAllTextAsync(TestConstants.TestSourceFiles.InteractorWithMissingInteractorSuffixFile, TestContext.Current.CancellationToken)
        };

        var project = AddDocumentsToTestProject(files);
        var compilation = await project.GetCompilationAsync(TestContext.Current.CancellationToken);
        Assert.NotNull(compilation);

        // act
        var diagnostics = await compilation
            .WithAnalyzers([new InteractorRequestModelValidationAnalyzer()]).GetAnalyzerDiagnosticsAsync(TestContext.Current.CancellationToken);

        // assert
        var missingInteractorSuffixError = Assert.Single(diagnostics);
        Assert.Equal("ST0003", missingInteractorSuffixError.Id);
        Assert.Equal("Test.cs", missingInteractorSuffixError.Location.GetMappedLineSpan().Path);
        Assert.Equal("Convention", missingInteractorSuffixError.Descriptor.Category);
        Assert.Equal(
            "GetFoobarInteractorWithoutProperSuffix must end with 'Interactor', since it implements 'IInteractor'",
            missingInteractorSuffixError.GetMessage());
    }

    private static Project CreateProject(string projectName)
    {
        var workspace = new AdhocWorkspace();

        var projectId = ProjectId.CreateNewId();
        var versionStamp = VersionStamp.Create();

        var projectInfo = ProjectInfo.Create(
            projectId,
            versionStamp,
            projectName,
            projectName,
            LanguageNames.CSharp,
            compilationOptions: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary),
            parseOptions: new CSharpParseOptions(LanguageVersion.Latest)
        );

        return workspace.AddProject(projectInfo)
            .AddMetadataReferences(GetDefaultReferences());
    }

    private static IEnumerable<MetadataReference> GetDefaultReferences()
    {
        var assemblies = new[]
        {
            typeof(object).Assembly, // System.Private.CoreLib
            typeof(Enumerable).Assembly, // System.Linq
            typeof(Debug).Assembly, // System.Diagnostics.Debug
            typeof(AssemblyTargetedPatchBandAttribute).Assembly, // System.Runtime
            typeof(AbstractValidator<>).Assembly, // FluentValidation
            typeof(OneOf<>).Assembly,
            typeof(UsedImplicitlyAttribute).Assembly // JetBrains.Annotations
        };

        return assemblies.Select(a => MetadataReference.CreateFromFile(a.Location));
    }
}