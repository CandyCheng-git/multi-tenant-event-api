namespace MultiTenantEventApi.Time;

public static class MelbourneTimeConverter
{
    private static readonly TimeZoneInfo Melbourne =
        TimeZoneInfo.FindSystemTimeZoneById("Australia/Melbourne");

    public static DateTime LocalToUtc(DateTime localDateTime)
    {
        var unspecified = DateTime.SpecifyKind(localDateTime, DateTimeKind.Unspecified);
        return TimeZoneInfo.ConvertTimeToUtc(unspecified, Melbourne);
    }
}
