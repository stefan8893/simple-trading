using System.Diagnostics;
using System.Runtime;
using AwesomeAssertions;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using OneOf;

namespace SimpleTrading.Domain.Analyzers.Tests;

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
    public async Task
        An_interactor_that_has_a_request_model_with_a_validator_must_have_a_bad_input_case_in_its_response_model()
    {
        // arrange
        var files = new Dictionary<string, string>
        {
            ["IInteractor.cs"] = TestConstants.InteractorSource,
            ["Test.cs"] = await File.ReadAllTextAsync(TestConstants.TestSourceFiles.ValidatorExistsForRequestModelFile)
        };

        var project = AddDocumentsToTestProject(files);
        var compilation = await project.GetCompilationAsync();
        compilation.Should().NotBeNull();

        // act
        var diagnostics = await compilation
            .WithAnalyzers([new InteractorRequestModelValidationAnalyzer()])
            .GetAnalyzerDiagnosticsAsync();

        // assert
        diagnostics.Should().HaveCount(1);
        var missingBadInputCaseError = diagnostics[0];
        missingBadInputCaseError.Id.Should().Be("ST0001");
        missingBadInputCaseError.Location.GetMappedLineSpan().Path.Should().Be("Test.cs");
        missingBadInputCaseError.Descriptor.Category.Should().Be("Usage");
        missingBadInputCaseError.GetMessage().Should()
            .Be(
                "Response model type 'OneOf' does not contain a case for 'BadInput', but this is required since there is a validator for 'GetFoobarRequestModel'");
    }

    [Fact]
    public async Task
        An_interactor_that_has_a_request_model_with_a_validator_and_a_response_model_that_is_not_of_type_OneOf_results_in_an_error()
    {
        // arrange
        var files = new Dictionary<string, string>
        {
            ["IInteractor.cs"] = TestConstants.InteractorSource,
            ["Test.cs"] = await File.ReadAllTextAsync(TestConstants.TestSourceFiles.ResponseModelTypeIsNotOneOfFile)
        };

        var project = AddDocumentsToTestProject(files);
        var compilation = await project.GetCompilationAsync();
        compilation.Should().NotBeNull();

        // act
        var diagnostics = await compilation
            .WithAnalyzers([new InteractorRequestModelValidationAnalyzer()])
            .GetAnalyzerDiagnosticsAsync();

        // assert
        diagnostics.Should().HaveCount(1);
        var responseTypeMustBeOneOfError = diagnostics[0];
        responseTypeMustBeOneOfError.Id.Should().Be("ST0002");
        responseTypeMustBeOneOfError.Location.GetMappedLineSpan().Path.Should().Be("Test.cs");
        responseTypeMustBeOneOfError.Descriptor.Category.Should().Be("Usage");
        responseTypeMustBeOneOfError.GetMessage().Should()
            .Be(
                "Response model must be of type 'OneOf' including a 'BadInput' case, because there is a validator for 'GetFoobarRequestModel'");
    }

    [Fact]
    public async Task
        An_interactor_that_has_a_request_model_with_a_validator_and_a_response_model_with_a_bad_input_case_does_not_result_in_an_error()
    {
        // arrange
        var files = new Dictionary<string, string>
        {
            ["IInteractor.cs"] = TestConstants.InteractorSource,
            ["Test.cs"] = await File.ReadAllTextAsync(TestConstants.TestSourceFiles.ValidatorAndBadInputCaseExistsFile)
        };

        var project = AddDocumentsToTestProject(files);
        var compilation = await project.GetCompilationAsync();
        compilation.Should().NotBeNull();

        // act
        var diagnostics = await compilation
            .WithAnalyzers([new InteractorRequestModelValidationAnalyzer()])
            .GetAnalyzerDiagnosticsAsync();

        // assert
        diagnostics.Should().BeEmpty();
    }

    [Fact]
    public async Task Interactor_names_must_end_with_Interactor()
    {
        // arrange
        var files = new Dictionary<string, string>
        {
            ["IInteractor.cs"] = TestConstants.InteractorSource,
            ["Test.cs"] =
                await File.ReadAllTextAsync(TestConstants.TestSourceFiles.InteractorWithMissingInteractorSuffixFile)
        };

        var project = AddDocumentsToTestProject(files);
        var compilation = await project.GetCompilationAsync();
        compilation.Should().NotBeNull();

        // act
        var diagnostics = await compilation
            .WithAnalyzers([new InteractorRequestModelValidationAnalyzer()])
            .GetAnalyzerDiagnosticsAsync();

        // assert
        diagnostics.Should().HaveCount(1);
        var missingInteractorSuffixError = diagnostics[0];
        missingInteractorSuffixError.Id.Should().Be("ST0003");
        missingInteractorSuffixError.Location.GetMappedLineSpan().Path.Should().Be("Test.cs");
        missingInteractorSuffixError.Descriptor.Category.Should().Be("Convention");
        missingInteractorSuffixError.GetMessage().Should()
            .Be("GetFoobarInteractorWithoutProperSuffix must end with 'Interactor', since it implements 'IInteractor'");
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