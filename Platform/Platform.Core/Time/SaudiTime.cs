namespace Platform.Core.Time;

public static class SaudiTime
{
    private static readonly TimeZoneInfo SaudiTimeZone =
        TimeZoneInfo.FindSystemTimeZoneById("Asia/Riyadh");

    public static DateTime Now =>
        TimeZoneInfo.ConvertTimeFromUtc(
            DateTime.UtcNow,
            SaudiTimeZone);
}