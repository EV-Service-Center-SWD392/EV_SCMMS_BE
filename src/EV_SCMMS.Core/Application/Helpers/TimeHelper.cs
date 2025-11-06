using System.Globalization;

public class TimeHelper
{
  public static bool BeValidDateFormat(string? dateString)
  {
    if (string.IsNullOrWhiteSpace(dateString)) return false;
    return DateOnly.TryParseExact(dateString, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
  }

  /// <summary>
  /// Validate time string exact format "HH:mm" (thống nhất, không giây)
  /// </summary>
  public static bool BeValidTimeFormat(string? timeString)
  {
    if (string.IsNullOrWhiteSpace(timeString)) return false;
    return TimeOnly.TryParseExact(timeString, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
  }
}
