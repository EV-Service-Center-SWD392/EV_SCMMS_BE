namespace EV_SCMMS.Core.Application.Events;

public class PayOsPaymentResultEvent
{
    public int PaymentId { get; init; }
    public bool IsSuccess { get; init; }
    public int Amount { get; init; }
    public string? Reference { get; init; }
}
