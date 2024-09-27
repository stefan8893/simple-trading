using Buildalyzer;
using Buildalyzer.Workspaces;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace SimpleTrading.Domain.Analyzers.Tests;

public class InteractorRequestModelValidationAnalyzerTests
{
    private readonly Project _testProject;

    public InteractorRequestModelValidationAnalyzerTests()
    {
        var manager = new AnalyzerManager();
        var analyzer = manager.GetProject(TestConstants.PathToTestCasesProjectFile);
        var workspace = new AdhocWorkspace();

        _testProject = analyzer.AddToWorkspace(workspace);
    }

    [Fact]
    public async Task An_interactor_that_has_a_request_model_with_a_validator_and_a_response_model_that_is_not_of_type_OneOf_results_in_an_error()
    {
        // arrange
        var cSharpTestFile = await File.ReadAllTextAsync(TestConstants.TestCases.ResponseModelTypeIsNotOneOfCase);
        var document = _testProject.AddDocument("Test.cs", cSharpTestFile);
        
        var compilation = await document.Project.GetCompilationAsync();
        compilation.Should().NotBeNull();

        // act
        var diagnostics = await compilation!
            .WithAnalyzers([new InteractorRequestModelValidationAnalyzer()])
            .GetAnalyzerDiagnosticsAsync();

        // assert
        diagnostics.Should().HaveCount(1);
        var responseTypeMustBeOneOfError = diagnostics[0];
        responseTypeMustBeOneOfError.Id.Should().Be("ST0002");
        responseTypeMustBeOneOfError.Location.GetMappedLineSpan().Path.Should().Be("Test.cs");
        responseTypeMustBeOneOfError.GetMessage().Should()
            .Be(
                "Response model must be of type 'OneOf' including a 'BadInput' case, because there is a validator for 'GetFoobarRequestModel'");
    }

    [Fact]
    public async Task An_interactor_that_has_a_request_model_with_a_validator_and_a_response_model_with_a_bad_input_case_does_not_result_in_an_error()
    {
        // arrange
        var cSharpTestFile = await File.ReadAllTextAsync(TestConstants.TestCases.ValidatorAndBadInputCaseExistsCase);
        var document = _testProject.AddDocument("Test.cs", cSharpTestFile);
        
        var compilation = await document.Project.GetCompilationAsync();
        compilation.Should().NotBeNull();

        // act
        var diagnostics = await compilation!
            .WithAnalyzers([new InteractorRequestModelValidationAnalyzer()])
            .GetAnalyzerDiagnosticsAsync();

        // assert
        diagnostics.Should().BeEmpty();
    }
    
    [Fact]
    public async Task
        An_interactor_that_has_a_request_model_with_a_validator_must_have_a_bad_input_case_in_its_response_model()
    {
        // arrange
        var cSharpTestFile = await File.ReadAllTextAsync(TestConstants.TestCases.ValidatorExistsForRequestModelCase);
        var document = _testProject.AddDocument("Test.cs", cSharpTestFile);
        
        var compilation = await document.Project.GetCompilationAsync();
        compilation.Should().NotBeNull();

        // act
        var diagnostics = await compilation!
            .WithAnalyzers([new InteractorRequestModelValidationAnalyzer()])
            .GetAnalyzerDiagnosticsAsync();

        // assert
        diagnostics.Should().HaveCount(1);
        var missingBadInputCaseError = diagnostics[0];
        missingBadInputCaseError.Id.Should().Be("ST0001");
        missingBadInputCaseError.Location.GetMappedLineSpan().Path.Should().Be("Test.cs");
        missingBadInputCaseError.GetMessage().Should()
            .Be(
                "Response model type 'OneOf' does not contain a case for 'BadInput', but this is required since there is a validator for 'GetFoobarRequestModel'");
    }
}