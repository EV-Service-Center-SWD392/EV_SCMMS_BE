using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EV_SCMMS.WebAPI.Converters;

/// <summary>
/// Custom JSON converter to handle multiple DateTime formats
/// Supports both ISO 8601 and common formats like "yyyy-MM-dd HH:mm:ss"
/// </summary>
public class FlexibleDateTimeConverter : JsonConverter<DateTime>
{
    private static readonly string[] SupportedFormats = new[]
    {
        "yyyy-MM-dd HH:mm:ss",           // 2025-11-09 13:00:00
        "yyyy-MM-dd'T'HH:mm:ss",         // 2025-11-09T13:00:00
        "yyyy-MM-dd'T'HH:mm:ss'Z'",      // 2025-11-09T13:00:00Z
        "yyyy-MM-dd'T'HH:mm:ss.fff",     // 2025-11-09T13:00:00.123
        "yyyy-MM-dd'T'HH:mm:ss.fff'Z'",  // 2025-11-09T13:00:00.123Z
        "yyyy-MM-dd'T'HH:mm:ssK",        // ISO 8601 with timezone
        "yyyy-MM-dd'T'HH:mm:ss.fffK",    // ISO 8601 with milliseconds and timezone
        "yyyy-MM-ddTHH:mm:ss.fffffffK",  // Full precision ISO 8601
        "O",                              // Round-trip format
        "s"                               // Sortable format
    };

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var dateString = reader.GetString();

        if (string.IsNullOrWhiteSpace(dateString))
        {
            throw new JsonException("DateTime string is null or empty");
        }

        // Try parsing with each supported format
        foreach (var format in SupportedFormats)
        {
            if (DateTime.TryParseExact(dateString, format, CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var result))
            {
                return result;
            }
        }

        // If none of the formats worked, try general parsing
        if (DateTime.TryParse(dateString, CultureInfo.InvariantCulture,
            DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var generalResult))
        {
            return generalResult;
        }

        throw new JsonException($"Unable to parse DateTime from '{dateString}'. " +
            $"Supported formats include: 'yyyy-MM-dd HH:mm:ss', 'yyyy-MM-ddTHH:mm:ssZ', etc.");
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        // Write in ISO 8601 format with UTC indicator
        writer.WriteStringValue(value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture));
    }
}
