using System.Reflection.Metadata;

public class BookingStatusConstant
{

  // Order
  public const string Pending = "PENDING";
  public const string Requested = "REQUESTED";
  public const string Rejected = "REJECTED";
  public const string Approved = "APPROVED";
  public const string InProgress = "IN_PROGRESS";

  public const string Canceled = "CANCELED";
  public static readonly string[] BookingStatus =
  { Approved, InProgress, Pending, Rejected, Requested, Canceled };

  public static readonly string[] ProcessingBookingRecord = { Approved, InProgress, Pending, Requested };

  public static readonly string[] PendingStatuses = { Pending, Requested };

  public static bool IsPending(string? status)
  {
    if (string.IsNullOrWhiteSpace(status)) return false;
    return PendingStatuses.Contains(status, StringComparer.OrdinalIgnoreCase);
  }

}
