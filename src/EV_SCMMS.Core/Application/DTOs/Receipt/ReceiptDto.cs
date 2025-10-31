using System;
using System.Collections.Generic;

namespace EV_SCMMS.Core.Application.DTOs.Receipt;

public class ReceiptDto
{
    public Guid ReceiptId { get; set; }
    public decimal TotalAmount { get; set; }
    public Guid CustomerId { get; set; }
    public Guid? TransactionId { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<ReceiptItemDto> Items { get; set; } = new();
}
