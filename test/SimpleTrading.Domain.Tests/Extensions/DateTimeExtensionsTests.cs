using FluentAssertions;
using SimpleTrading.Domain.Infrastructure.Extensions;

namespace SimpleTrading.Domain.Tests.Extensions;

public class DateTimeExtensionsTests
{
    [Fact]
    public void DateTime_gets_correctly_converted()
    {
        var utc = DateTime.Parse("2024-08-03T18:00:00").ToUtcKind();

        var newYork = utc.ToLocal("America/New_York");

        var expected = DateTimeOffset.Parse("2024-08-03T14:00:00-04:00");
        newYork.Should().Be(expected);
    }

    [Fact]
    public void Passing_an_unknown_timeZone_down_throws_an_Exception()
    {
        var utc = DateTime.Parse("2024-08-03T18:00:00").ToUtcKind();

        var act = () => utc.ToLocal("FooBar");

        act.Should().ThrowExactly<TimeZoneNotFoundException>();
    }

    [Theory]
    [InlineData(DateTimeKind.Local)]
    [InlineData(DateTimeKind.Unspecified)]
    public void Only_utc_dateTimes_get_converted(DateTimeKind kind)
    {
        var utc = DateTime.SpecifyKind(DateTime.Parse("2024-08-03T18:00:00"), kind);

        var act = () => utc.ToLocal("Europe/Vienna");

        act.Should().ThrowExactly<ArgumentException>().WithMessage("The given dateTime is not in UTC");
    }

    [Theory]
    [InlineData(DateTimeKind.Local)]
    [InlineData(DateTimeKind.Unspecified)]
    [InlineData(DateTimeKind.Utc)]
    public void ToUnspecifiedKind_converts_kind_properly(DateTimeKind kind)
    {
        var dateTime = new DateTime(2024, 8, 5, 12, 0, 0, kind);

        var unspecifiedDateTime = dateTime.ToUnspecifiedKind();

        unspecifiedDateTime.Kind.Should().Be(DateTimeKind.Unspecified);
    }
}