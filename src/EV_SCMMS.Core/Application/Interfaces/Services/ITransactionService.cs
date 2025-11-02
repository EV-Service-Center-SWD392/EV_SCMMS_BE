using EV_SCMMS.Core.Application.DTOs.Payment;
using EV_SCMMS.Core.Application.Results;
using Net.payOS.Types;

namespace EV_SCMMS.Core.Application.Interfaces.Services;

public interface ITransactionService
{
  Task<IServiceResult<TransactionDto>> CreateAsync(CreateTransactionDto createDto);
  Task<IServiceResult<TransactionDto>> GetByIdAsync(Guid id);
  Task<IServiceResult<TransactionDto>> UpdateStatusAsync(Guid id, UpdateTransactionStatusDto updateDto);
  Task<IServiceResult<bool>> DeleteAsync(Guid id);

  Task<IServiceResult<List<TransactionDto>>> GetAllByUserIdAsync(Guid userId);
  Task<IServiceResult<List<TransactionDto>>> GetAllByOrderIdAsync(Guid orderId);
  Task<IServiceResult<List<TransactionDto>>> GetAllAsync();

  // PayOS specific
  Task<CreatePaymentResult> CreatePayOsPaymentLink(PaymentData paymentData);

  // New: webhook and payos operations
  Task<IServiceResult<bool>> HandlePayOsWebhookAsync(WebhookType webhook);
  Task<IServiceResult<bool>> ConfirmWebhookAsync(string webhookUrl);
  Task<IServiceResult<bool>> CancelPayOsPaymentLinkAsync(int orderCode);

  Task<IServiceResult<TransactionDto?>> getByOrderCodeAsync(int orderCode);
}
