namespace SimpleTrading.Domain.Extensions;

public static class DateTimeExtensions
{
    public static DateTime ToLocal(this DateTime dateTime, string timeZone)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(timeZone, nameof(timeZone));

        if (dateTime.Kind != DateTimeKind.Utc)
            throw new ArgumentException("The given dateTime is not in UTC");

        var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZone);

        return TimeZoneInfo.ConvertTime(dateTime, timeZoneInfo).ToLocalKind();
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