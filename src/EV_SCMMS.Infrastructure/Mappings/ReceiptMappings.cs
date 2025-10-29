using EV_SCMMS.Core.Application.DTOs.Receipt;
using EV_SCMMS.Core.Domain.Models;

namespace EV_SCMMS.Infrastructure.Mappings;

public static class ReceiptMappings
{
    public static ReceiptDto ToDto(this Receiptcuongtq r)
    {
        if (r == null) return null!;
        return new ReceiptDto
        {
            ReceiptId = r.Receiptid,
            TotalAmount = r.Totalamount,
            CustomerId = r.Customerid,
            TransactionId = r.Transactionid,
            CreatedAt = r.Createdat,
            UpdatedAt = r.Updatedat,
            Items = r.Receiptitemcuongtqs?.Select(i => i.ToDto()).ToList() ?? new List<ReceiptItemDto>()
        };
    }

    public static ReceiptItemDto ToDto(this Receiptitemcuongtq i)
    {
        if (i == null) return null!;
        return new ReceiptItemDto
        {
            ReceiptItemId = i.Receiptitemid,
            ItemId = i.Itemid,
            ItemType = i.Itemtype,
            ItemName = i.Itemname,
            Quantity = i.Quantity,
            UnitPrice = i.Unitprice,
            LineTotal = i.Linetotal
        };
    }
}
