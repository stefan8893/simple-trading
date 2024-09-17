namespace SimpleTrading.Domain.Extensions;

public static class DateTimeExtensions
{
    public static DateTimeOffset ToLocal(this DateTime dateTime, string timeZone)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(timeZone, nameof(timeZone));

        if (dateTime.Kind != DateTimeKind.Utc)
            throw new ArgumentException("The given DateTime is not in UTC");

        var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
        var dateTimeOffset = new DateTimeOffset(dateTime);

        return TimeZoneInfo.ConvertTime(dateTimeOffset, timeZoneInfo);
    }

    public static DateTime ToUnspecifiedKind(this DateTime dateTime)
    {
        return DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified);
    }

    public static DateTime ToUtcKind(this DateTime dateTime)
    {
        return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
    }

    public static DateTime ToLocalKind(this DateTime dateTime)
    {
        return DateTime.SpecifyKind(dateTime, DateTimeKind.Local);
    }
}