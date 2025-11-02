using EV_SCMMS.Core.Application.DTOs.Payment;
using EV_SCMMS.Core.Domain.Models;

namespace EV_SCMMS.Infrastructure.Mappings;

public static class TransactionMapper
{
  public static Transactioncuongtq ToEntity(this CreateTransactionDto dto, decimal totalAmount)
  {
    return new Transactioncuongtq
    {
      Transactionid = Guid.NewGuid(),
      Orderid = dto.OrderId,
      Staffid = dto.StaffId,
      Paymentmethodid = dto.PaymentMethodId,
      Totalamount = totalAmount,
      Createdat = DateTime.UtcNow,
      Updatedat = null
    };
  }

  public static TransactionDto? ToDto(this Transactioncuongtq entity)
  {
    if (entity == null) return null;

    return new TransactionDto
    {
      TransactionId = entity.Transactionid,
      OrderId = entity.Orderid,
      PaymentMethodId = entity.Paymentmethodid, 
      PaymentMethodName = entity.Paymentmethod?.Name,
      ReceiptId = entity.Receiptcuongtqs != null && entity.Receiptcuongtqs.Any()
        ? entity.Receiptcuongtqs.First().Receiptid.ToString()
        : null,
      TotalAmount = entity.Totalamount,
      Description = entity.Description,
      Reason = entity.Reason,
      StaffId = entity.Staffid,
      CreatedAt = entity.Createdat,
      UpdatedAt = entity.Updatedat,
      Status = entity.Status,
      Paymentlink = entity.Paymentlink,
      PaymentId = entity.Paymentid
    };
  }
}
