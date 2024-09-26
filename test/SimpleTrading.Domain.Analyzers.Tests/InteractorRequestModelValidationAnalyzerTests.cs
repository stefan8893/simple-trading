using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;

namespace SimpleTrading.Domain.Analyzers.Tests;

using Verify = CSharpAnalyzerVerifier<
    InteractorRequestModelValidationAnalyzer,
    DefaultVerifier>;

public class InteractorRequestModelValidationAnalyzerTests
{
    [Fact]
    public async Task HappyPath()
    {
        const string sourceCode = @"";

        var expected = Verify.Diagnostic(Rules.MissingBadInputCase)
            .WithLocation(7, 28)
            .WithArguments("300000000");

        await Verify.VerifyAnalyzerAsync(sourceCode, expected);
    }
}