using System.Globalization;

namespace webVentasSaaSV25.Helpers;

public static class MxTime
{
    private static readonly CultureInfo Culture = CultureInfo.GetCultureInfo("es-MX");
    private static readonly TimeZoneInfo Zone = ResolveZone();

    public static System.DateTime FromUtc(System.DateTime utc)
    {
        var normalized = utc.Kind switch
        {
            DateTimeKind.Utc => utc,
            DateTimeKind.Local => utc.ToUniversalTime(),
            _ => System.DateTime.SpecifyKind(utc, DateTimeKind.Utc)
        };

        return TimeZoneInfo.ConvertTimeFromUtc(normalized, Zone);
    }

    public static string Date(System.DateTime utc)
        => FromUtc(utc).ToString("dd/MM/yyyy", Culture);

    public static string Date(System.DateTime? utc)
        => utc.HasValue ? Date(utc.Value) : "-";

    public static string DateTime(System.DateTime utc)
        => FromUtc(utc).ToString("dd/MM/yyyy HH:mm", Culture);

    public static string DateTime(System.DateTime? utc)
        => utc.HasValue ? DateTime(utc.Value) : "-";

    public static string DateTimeShort(System.DateTime utc)
        => FromUtc(utc).ToString("dd/MM/yy HH:mm", Culture);

    public static string DateTimeShort(System.DateTime? utc)
        => utc.HasValue ? DateTimeShort(utc.Value) : "-";

    public static string DateTimeSeconds(System.DateTime utc)
        => FromUtc(utc).ToString("dd/MM/yyyy HH:mm:ss", Culture);

    public static string DateOnly(System.DateTime date)
        => date.ToString("dd/MM/yyyy", Culture);

    public static string DateOnly(System.DateTime? date)
        => date.HasValue ? DateOnly(date.Value) : "-";

    private static TimeZoneInfo ResolveZone()
    {
        foreach (var id in new[] { "America/Mexico_City", "Central Standard Time (Mexico)", "Central Standard Time" })
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById(id);
            }
            catch
            {
                // Try the next known id.
            }
        }

        return TimeZoneInfo.Local;
    }
}
