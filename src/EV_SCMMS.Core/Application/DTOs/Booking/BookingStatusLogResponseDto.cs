public class BookingStatusLogResponseDto
{
  public Guid LogId { get; set; }
  public Guid BookingId { get; set; }
  public string Status { get; set; } = default!;
  public DateTime? CreatedAt { get; set; }
  public DateTime? UpdatedAt { get; set; }

  // Extra info from Booking
  public Guid CustomerId { get; set; }
  public Guid VehicleId { get; set; }
  public Guid? SlotId { get; set; }
  public string? Notes { get; set; }

  public bool IsSeen { get; set; }
}

public class BookingStatusLogResponseWithUnseenLogCount
{
  public int UnseenLogsCount { get; set; }
  public List<BookingStatusLogResponseDto> Result { get; set; }


}
