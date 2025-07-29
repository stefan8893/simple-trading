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
        Assert.Equal(expected, newYork);
    }

    [Fact]
    public void Passing_an_unknown_timeZone_down_throws_an_Exception()
    {
        var utc = DateTime.Parse("2024-08-03T18:00:00").ToUtcKind();

        // ReSharper disable once MoveLocalFunctionAfterJumpStatement
        void Act() => utc.ToLocal("FooBar");

        Assert.Throws<TimeZoneNotFoundException>(Act);
    }

    [Theory]
    [InlineData(DateTimeKind.Local)]
    [InlineData(DateTimeKind.Unspecified)]
    public void Only_utc_dateTimes_get_converted(DateTimeKind kind)
    {
        var utc = DateTime.SpecifyKind(DateTime.Parse("2024-08-03T18:00:00"), kind);

        var exception = Assert.Throws<ArgumentException>(() => utc.ToLocal("Europe/Vienna"));
        Assert.Equal("The given DateTime is not in UTC", exception.Message);
    }

    [Theory]
    [InlineData(DateTimeKind.Local)]
    [InlineData(DateTimeKind.Unspecified)]
    [InlineData(DateTimeKind.Utc)]
    public void ToUnspecifiedKind_converts_kind_properly(DateTimeKind kind)
    {
        var dateTime = new DateTime(2024, 8, 5, 12, 0, 0, kind);

        var unspecifiedDateTime = dateTime.ToUnspecifiedKind();

        Assert.Equal(DateTimeKind.Unspecified, unspecifiedDateTime.Kind);
    }
}