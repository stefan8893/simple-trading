namespace SimpleTrading.Domain.Infrastructure.Extensions;

public static class DateTimeExtensions
{
    extension(DateTime dateTime)
    {
        public DateTimeOffset ToLocal(string timeZone)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(timeZone, nameof(timeZone));

            if (dateTime.Kind != DateTimeKind.Utc)
                throw new ArgumentException("The given DateTime is not in UTC");

            var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
            var dateTimeOffset = new DateTimeOffset(dateTime);

            return TimeZoneInfo.ConvertTime(dateTimeOffset, timeZoneInfo);
        }

        public DateTime ToUnspecifiedKind()
        {
            return DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified);
        }

        public DateTime ToUtcKind()
        {
            return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
        }
    }
}