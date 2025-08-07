using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;

namespace SimpleTrading.Domain.Generators.Tests;

public class InteractorDiagnosticsTests
{
    [Fact]
    public async Task Interactors_must_have_the_Interactor_suffix()
    {
        var expectedDiagnostic = new DiagnosticResult("ST0001", DiagnosticSeverity.Error)
            .WithMessage(
                "GetFoobarInteractorWithoutProperSuffix must end with 'Interactor', since it implements 'IInteractor'")
            .WithArguments("GetFoobarInteractorWithoutProperSuffix")
            .WithLocation("/0/Test0.cs", 14, 14);

        var diagnosticsTest = new CSharpSourceGeneratorTest<InteractorProxyGenerator, XunitV3Verifier>
        {
            TestCode = TestSources.InteractorWithMissingInteractorSuffix,
            ExpectedDiagnostics = {expectedDiagnostic}
        };

        diagnosticsTest.TestBehaviors |= TestBehaviors.SkipGeneratedSourcesCheck;

        await diagnosticsTest.RunAsync(TestContext.Current.CancellationToken);
    }
}