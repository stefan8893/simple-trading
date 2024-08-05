using FluentAssertions;
using SimpleTrading.Domain.Extensions;

namespace SimpleTrading.Domain.Tests.Extensions;

public class DateTimeExtensionsTests
{
    [Fact]
    public void DateTime_gets_correctly_converted()
    {
        var utc = DateTime.Parse("2024-08-03T18:00:00").ToUtcKind();

        var newYork = utc.ToLocal("America/New_York");

        var expected = DateTime.Parse("2024-08-03T14:00:00");
        newYork.Should().Be(expected);
        newYork.Kind.Should().Be(DateTimeKind.Local);
    }

    [Fact]
    public void Passing_an_unknown_timeZone_down_throws_an_Exception()
    {
        var utc = DateTime.Parse("2024-08-03T18:00:00").ToUtcKind();

        var act = () => utc.ToLocal("FooBar");

        act.Should().Throw<TimeZoneNotFoundException>();
    }

    [Theory]
    [InlineData(DateTimeKind.Local)]
    [InlineData(DateTimeKind.Unspecified)]
    public void Only_utc_dateTimes_get_converted(DateTimeKind kind)
    {
        var utc = DateTime.SpecifyKind(DateTime.Parse("2024-08-03T18:00:00"), kind);

        var act = () => utc.ToLocal("Europe/Vienna");

        act.Should().Throw<ArgumentException>().WithMessage("The given dateTime is not in UTC");
    }
}