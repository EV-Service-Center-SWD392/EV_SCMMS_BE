namespace EV_SCMMS.Core.Application.DTOs.Payment;

public class TransactionDto
{
    public Guid TransactionId { get; set; }
    public Guid OrderId { get; set; }
    public int? PaymentMethodId { get; set; }
    public string? Description { get; set; }
    public string? Reason { get; set; }
    public decimal TotalAmount { get; set; }
    public string? Status { get; set; }
    public Guid? StaffId { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

  public string? Paymentlink { get; set; }
  public int? PaymentId { get; set; }
}
