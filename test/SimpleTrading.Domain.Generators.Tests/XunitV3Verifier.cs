using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;

namespace SimpleTrading.Domain.Generators.Tests;

public class XunitV3Verifier : IVerifier
{
    private readonly string? _context;

    public XunitV3Verifier()
    {
    }

    public XunitV3Verifier(string? context = null)
    {
        _context = context;
    }

    public void Empty<T>(string collectionName, IEnumerable<T> collection)
    {
        var firstFiveItems = collection.Take(5).ToList();

        if (firstFiveItems.Count == 0) return;

        var preview = string.Join(", ", firstFiveItems);
        Assert.Fail($"{collectionName} is not empty. Contents (up to 5 items): {preview}");
    }

    public void Equal<T>(T expected, T actual, string? message = null)
    {
        Assert.True(EqualityComparer<T>.Default.Equals(expected, actual),
            message ?? $"Expected: {expected}, Actual: {actual}");
    }

    public void True([DoesNotReturnIf(false)] bool assert, string? message = null)
    {
        Assert.True(assert, message);
    }

    public void False([DoesNotReturnIf(true)] bool assert, string? message = null)
    {
        Assert.False(assert, message);
    }

    [DoesNotReturn]
    public void Fail(string? message = null)
    {
        Assert.Fail(message);
    }

    public void LanguageIsSupported(string language)
    {
        if (language != LanguageNames.CSharp) Assert.Fail($"The language is not supported: {language}.");
    }

    public void NotEmpty<T>(string collectionName, IEnumerable<T> collection)
    {
        if (collection.Any()) return;

        Assert.Fail($"{collectionName} is empty.");
    }

    public void SequenceEqual<T>(IEnumerable<T> expected, IEnumerable<T> actual,
        IEqualityComparer<T>? equalityComparer = null,
        string? message = null)
    {
        if (equalityComparer is null)
        {
            Assert.Equal(expected, actual);

            return;
        }

        var expectedList = expected.ToList();
        var actualList = actual.ToList();

        if (expectedList.Count != actualList.Count)
            Assert.Fail(message ??
                        $"Sequences have different lengths: expected {expectedList.Count}, actual {actualList.Count}");

        for (var i = 0; i < expectedList.Count; i++)
            if (!equalityComparer.Equals(expectedList[i], actualList[i]))
                Assert.Fail(message ??
                            $"Sequences differ at index {i}: expected {expectedList[i]}, actual {actualList[i]}");
    }

    public IVerifier PushContext(string context1)
    {
        var newContext = _context is null ? context1 : $"{_context} > {context1}";
        return new XunitV3Verifier(newContext);
    }
}