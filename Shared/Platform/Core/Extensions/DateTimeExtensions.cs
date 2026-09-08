namespace Platform.Core.Extensions;

public static class DateTimeExtensions
{
    public static DateTime FromUtcToSaudiTime(this DateTime utcTime)
    {
        return TimeZoneInfo.ConvertTimeFromUtc(
            DateTime.SpecifyKind(utcTime, DateTimeKind.Utc),
            GetSaudiTimeZone());
    }

    public static DateTime FromSaudiTimeToUtc(this DateTime dateTime)
    {
        return TimeZoneInfo.ConvertTimeToUtc(
            DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified),
            GetSaudiTimeZone());
    }

    private static TimeZoneInfo GetSaudiTimeZone()
    {
        var timeZoneId = OperatingSystem.IsWindows()
            ? "Arab Standard Time"
            : "Asia/Riyadh";

        return TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
    }
}
