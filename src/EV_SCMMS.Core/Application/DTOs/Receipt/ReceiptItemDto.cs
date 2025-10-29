using System;

namespace EV_SCMMS.Core.Application.DTOs.Receipt;

public class ReceiptItemDto
{
    public Guid ReceiptItemId { get; set; }
    public Guid? ItemId { get; set; }
    public string? ItemName { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
}
