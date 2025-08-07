using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;

namespace SimpleTrading.Domain.Generators.Tests;

public class InteractorDiagnosticsTests
{
    [Fact]
    public void DummyTest_remove_me_when_other_tests_are_running()
    {
        Assert.True(true, "A Test project without a running test will fail");
    }
    
    [Fact(Skip = "Under Construction")]
    public async Task Interactors_must_have_the_Interactor_suffix()
    {
        var expectedDiagnostic = new DiagnosticResult("ST0001", DiagnosticSeverity.Error);
        expectedDiagnostic
            .WithSpan(14, 14, 14, 52)
            .WithMessage(
                "GetFoobarInteractorWithoutProperSuffix must end with 'Interactor', since it implements 'IInteractor'");

        var diagnosticsTest = new CSharpSourceGeneratorTest<InteractorProxyGenerator, XunitV3Verifier>
        {
            TestCode = TestSources.InteractorWithMissingInteractorSuffix,
            ExpectedDiagnostics = {expectedDiagnostic}
        };

        diagnosticsTest.TestBehaviors |= TestBehaviors.SkipGeneratedSourcesCheck;

        await diagnosticsTest.RunAsync(TestContext.Current.CancellationToken);
    }
}