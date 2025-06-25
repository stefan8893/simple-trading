using AwesomeAssertions;
using SimpleTrading.Domain.Trading;
using SimpleTrading.TestInfrastructure;

namespace SimpleTrading.Domain.Tests.Trading;

public class ResultTests : TestBase
{
    [Fact]
    public void Creating_a_result_with_a_not_supported_name_throws_an_argument_exceptions()
    {
        const string notSupportedName = "NotThatBad";

        var act = () => new Result(notSupportedName, ResultSource.ManuallyEntered);

        var exception = act.Should().ThrowExactly<ArgumentException>();
        exception.Which.Message.Should()
            .Be("Invalid result name. It must be one of 'Loss, BreakEven, Mediocre, Win'");
    }

    [Theory]
    [InlineData("WiN", Result.Win)]
    [InlineData("win", Result.Win)]
    [InlineData("mEdIoCrE", Result.Mediocre)]
    [InlineData("LOss", Result.Loss)]
    [InlineData("breakEven", Result.BreakEven)]
    public void Creating_a_result_with_different_name_casings_results_always_in_a_capitalized_name(
        string nameCandidate, string expectedName)
    {
        var result = new Result(nameCandidate, ResultSource.ManuallyEntered);

        result.Name.Should().Be(expectedName);
    }

    [Theory]
    [InlineData("Win ")]
    [InlineData(" Win")]
    [InlineData(" Win ")]
    [InlineData(" Win    ")]
    public void Leading_and_trailing_whitespaces_will_be_removed_from_the_name(string name)
    {
        var result = new Result(name, ResultSource.ManuallyEntered);

        result.Name.Should().Be(Result.Win);
    }
}