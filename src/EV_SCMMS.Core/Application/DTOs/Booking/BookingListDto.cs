public class BookingWithSlotDto
{
  public Guid BookingId { get; set; }
  public Guid CustomerId { get; set; }
  public Guid VehicleId { get; set; }
  public Guid? SlotId { get; set; }
  public string? Notes { get; set; }
  public string? Status { get; set; }
  public bool? IsActive { get; set; }
  public DateOnly BookingDate { get; set; }
  public DateTime? CreatedAt { get; set; }
  public DateTime? UpdatedAt { get; set; }

  public BookingSlotDto? Slot { get; set; }
}
