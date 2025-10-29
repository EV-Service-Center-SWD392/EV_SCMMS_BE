using EV_SCMMS.Core.Application.DTOs.Payment;
using EV_SCMMS.Core.Application.Interfaces;
using EV_SCMMS.Core.Application.Interfaces.Repositories;
using EV_SCMMS.Core.Application.Interfaces.Services;
using EV_SCMMS.Core.Application.Results;
using EV_SCMMS.Core.Application.Enums;
using EV_SCMMS.Infrastructure.Mappings;
using Net.payOS.Types;
using Microsoft.Extensions.Options;
using EV_SCMMS.Infrastructure.Configuration;
using EV_SCMMS.Core.Application.Events;
using EV_SCMMS.Core.Domain.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace EV_SCMMS.Infrastructure.Services;

public class TransactionService : ITransactionService
{
  private readonly IUnitOfWork _unitOfWork;
  private readonly IPayOsClient _payOsClient;
  private readonly PayOsOptions _payOsOptions;
  private readonly IEventPublisher _eventPublisher;
  private readonly ICurrentUserService _currentUser;

  public TransactionService(IUnitOfWork unitOfWork, IPayOsClient payOsClient, IOptions<PayOsOptions> payOsOptions, IEventPublisher eventPublisher, ICurrentUserService currentUser)
  {
    _unitOfWork = unitOfWork;
    _payOsClient = payOsClient;
    _payOsOptions = payOsOptions.Value;
    _eventPublisher = eventPublisher;
    _currentUser = currentUser;

    // subscribe internal handler for PayOsPaymentResultEvent
    _eventPublisher.Subscribe<PayOsPaymentResultEvent>(HandlePayOsPaymentResultEventAsync);
  }

  private async Task HandlePayOsPaymentResultEventAsync(PayOsPaymentResultEvent evt)
  {
    // idempotent update: find transaction and update if necessary
    var entity = await _unitOfWork.TransactionRepository.GetByPaymentIdAsync(evt.PaymentId);
    if (entity == null) return;

    // if already paid and event is success, ignore
    if (string.Equals(entity.Status, TransactionStatus.PAID.ToString(), StringComparison.OrdinalIgnoreCase) && evt.IsSuccess) return;

    entity.Status = evt.IsSuccess ? TransactionStatus.PAID.ToString() : TransactionStatus.CANCELLED.ToString();

    await _unitOfWork.TransactionRepository.UpdateAsync(entity);

    // If paid, create receipt and receipt items for order services and spareparts
    if (evt.IsSuccess)
    {
      await CreateReceiptForTransactionAsync(entity);
    }

    await _unitOfWork.SaveChangesAsync();
  }

  // Extracted helper to create receipt and receipt items for a transaction
  private async Task CreateReceiptForTransactionAsync(Transactioncuongtq entity)
  {
    if (entity == null) return;

    // ensure we don't create duplicate receipts for the same transaction
    if (entity.Transactionid != Guid.Empty)
    {
      var exists = await _unitOfWork.ReceiptRepository.GetAllQueryable()
        .AnyAsync(r => r.Transactionid == entity.Transactionid);
      if (exists) return;
    }

    // retrieve order with items
    var orderWithItems = await _unitOfWork.OrderRepository.GetByIdWithItemsAsync(entity.Orderid);
    if (orderWithItems == null) return;

    // create receipt
    var receipt = new Receiptcuongtq
    {
      Receiptid = Guid.NewGuid(),
      Totalamount = entity.Totalamount,
      Customerid = orderWithItems.Customerid,
      Transactionid = entity.Transactionid,
    };

    await _unitOfWork.ReceiptRepository.AddAsync(receipt);

    // services
    foreach (var s in orderWithItems.Orderservicethaontts)
    {
      var qty = s.Quantity ?? 1;
      var unitPrice = s.Unitprice ?? 0m;
      var lineTotal = qty * unitPrice;

      var item = new Receiptitemcuongtq
      {
        Receiptitemid = Guid.NewGuid(),
        Receiptid = receipt.Receiptid,
        Itemname = s.Service?.Name ?? string.Empty,
        Itemtype = ItemType.SERVICE.ToString(),
        Itemid = s.Serviceid,
        Quantity = qty,
        Unitprice = unitPrice,
        Linetotal = lineTotal,
      };

      await _unitOfWork.ReceiptItemRepository.AddAsync(item);
    }

    // spareparts
    foreach (var p in orderWithItems.Orderspareparts)
    {
      var qty = p.Quantity;
      var unitPrice = p.Unitprice ?? 0m;
      var lineTotal = qty * unitPrice;

      var item = new Receiptitemcuongtq
      {
        Receiptitemid = Guid.NewGuid(),
        Receiptid = receipt.Receiptid,
        Itemname = p.Sparepart?.Name ?? string.Empty,
        Itemid = p.Sparepartid,
        Itemtype = ItemType.SPAREPART.ToString(),
        Quantity = qty,
        Unitprice = unitPrice,
        Linetotal = lineTotal,
      };

      await _unitOfWork.ReceiptItemRepository.AddAsync(item);
    }

    // save created receipt and items
    await _unitOfWork.SaveChangesAsync();
  }

  public async Task<IServiceResult<TransactionDto>> CreateAsync(CreateTransactionDto createDto)
  {
    // ensure order exists and use its TotalAmount
    var order = await _unitOfWork.OrderRepository.GetByIdAsync(createDto.OrderId);
    if (order == null)
      return ServiceResult<TransactionDto>.Failure("Order not found");

    var total = order.Totalamount ?? 0m;

    var entity = createDto.ToEntity(total);

    // If PaymentMethodId == 2 then create PayOS payment link and attach to transaction
    if (createDto.PaymentMethodId == 2)
    {
      try
      {
        // create orderCode as demo code (same pattern used elsewhere)
        int orderCode = int.Parse(DateTimeOffset.Now.ToString("ffffff"));

        // build PaymentData - convert total to int (adapter/PayOS seems to use int price)
        var priceInt = (int)Math.Round(total);

        var item = new ItemData($"Order {order.Orderid}", 1, priceInt);
        var items = new List<ItemData> { item };

        // Use return/cancel URLs from options
        var cancelUrl = _payOsOptions.CancelUrl ?? string.Empty;
        var returnUrl = _payOsOptions.ReturnUrl ?? string.Empty;

        var paymentData = new PaymentData(orderCode, priceInt, $"Payment for order Testing", items, cancelUrl, returnUrl);

        var createPayment = await _payOsClient.createPaymentLink(paymentData);

        // attach payos data to transaction entity
        entity.Paymentid = orderCode;

        // try to extract checkoutUrl from createPayment via reflection to avoid coupling
        try
        {
          var checkoutUrlProp = createPayment.GetType().GetProperty("checkoutUrl") ?? createPayment.GetType().GetProperty("CheckoutUrl");
          if (checkoutUrlProp != null)
          {
            var url = checkoutUrlProp.GetValue(createPayment) as string;
            entity.Paymentlink = url;
          }
        }
        catch
        {
          // ignore
        }
      }
      catch (Exception ex)
      {
        return ServiceResult<TransactionDto>.Failure($"Failed to create PayOS payment link: {ex.Message}");
      }
    }

    await _unitOfWork.TransactionRepository.AddAsync(entity);
    await _unitOfWork.SaveChangesAsync();

    var dto = entity.ToDto();

    return ServiceResult<TransactionDto>.Success(dto);
  }

  public async Task<IServiceResult<TransactionDto>> GetByIdAsync(Guid id)
  {

    var entity = await _unitOfWork.TransactionRepository.GetByIdIncludeOrderAsync(id);
    if (entity == null)
      return ServiceResult<TransactionDto>.Failure("Transaction not found");
    if (_currentUser != null && _currentUser.Role == "CUSTOMER" && _currentUser.UserId.HasValue)
    {
      if (entity.Order.Customerid != _currentUser.UserId.Value)
        return ServiceResult<TransactionDto>.Failure("UnAuthorized");
    }
    var dto = entity.ToDto();

    return ServiceResult<TransactionDto>.Success(dto);
  }

  public async Task<IServiceResult<TransactionDto>> UpdateStatusAsync(Guid id, UpdateTransactionStatusDto updateDto)
  {
    var entity = await _unitOfWork.TransactionRepository.GetByIdAsync(id);
    if (entity == null)
      return ServiceResult<TransactionDto>.Failure("Transaction not found");
    if (entity.Status != "CREATED")
    {
      return ServiceResult<TransactionDto>.Failure("Status can only be updated to CREATED via this method");
    }
    if (!Enum.TryParse<TransactionStatus>(updateDto.Status, true, out var parsed))
    {
      return ServiceResult<TransactionDto>.Failure("Invalid status value");
    }
 
    entity.Status = parsed.ToString();

    await _unitOfWork.TransactionRepository.UpdateAsync(entity);

    // if status became PAID, create receipt
    if (parsed == TransactionStatus.PAID)
    {
      await CreateReceiptForTransactionAsync(entity);
    }

    await _unitOfWork.SaveChangesAsync();

    var dto = entity.ToDto();
    return ServiceResult<TransactionDto>.Success(dto);
  }

  public async Task<IServiceResult<bool>> DeleteAsync(Guid id)
  {
    var entity = await _unitOfWork.TransactionRepository.GetByIdIncludeOrderAsync(id);
    if (entity == null)
      return ServiceResult<bool>.Failure("Transaction not found");
    if (entity.Status != "CREATED")
    {
      return ServiceResult<bool>.Failure("Only transactions with status 'CREATED' can be cancelled");
    }
    entity.Reason = TransactionStatus.CANCELLED.ToString();

    if (_currentUser != null && _currentUser.UserId.HasValue)
    {
      if(entity.Order.Customerid != _currentUser.UserId.Value)
        return ServiceResult<bool>.Failure("UnAuthorized");
    }

    if (entity.Paymentid != 0)
    {
      try
      {
      var info = await _payOsClient.cancelPaymentLink(entity.Paymentid);
      }catch(Exception ex)
      {
        //ignore
        //return ServiceResult<bool>.Failure($"Failed to cancel PayOS payment link: {ex.Message}");
      }
    }
    await _unitOfWork.TransactionRepository.UpdateAsync(entity);
    await _unitOfWork.SaveChangesAsync();

    return ServiceResult<bool>.Success(true, "Transaction cancelled");
  }

  async Task<IServiceResult<List<TransactionDto>>> ITransactionService.GetAllByUserIdAsync(Guid userId)
  {
    if (_currentUser != null && _currentUser.UserId.HasValue && _currentUser.Role == "CUSTOMER")
    {
      if (userId != _currentUser.UserId.Value)
        return ServiceResult<List<TransactionDto>>.Failure("UnAuthorized");
    }
    var result = await _unitOfWork.TransactionRepository.GetAllByUserIdAsync(userId);
    return ServiceResult<List<TransactionDto>>.Success(result.Select(t => t.ToDto()).ToList());
  }
    
  async Task<IServiceResult<List<TransactionDto>>> ITransactionService.GetAllByOrderIdAsync(Guid orderId)
    {
    var result = await _unitOfWork.TransactionRepository.GetAllByOrderIdAsync(orderId);
    return ServiceResult<List<TransactionDto>>.Success(result.Select(t => t.ToDto()).ToList());
  }
     
  async Task<IServiceResult<List<TransactionDto>>> ITransactionService.GetAllAsync()
  {
    var result = await _unitOfWork.TransactionRepository.GetAllAsync();
    return ServiceResult<List<TransactionDto>>.Success(result.Select(t => t.ToDto()).ToList());
  }
  public async Task<CreatePaymentResult> CreatePayOsPaymentLink(PaymentData paymentData)
  {
    return await _payOsClient.createPaymentLink(paymentData);
  }

  // Confirm webhook URL with PayOS
  public async Task<IServiceResult<bool>> ConfirmWebhookAsync(string webhookUrl)
  {
    try
    {
      await _payOsClient.confirmWebhook(webhookUrl);
      return ServiceResult<bool>.Success(true);
    }
    catch (Exception ex)
    {
      return ServiceResult<bool>.Failure($"Confirm webhook failed: {ex.Message}");
    }
  }

  // Cancel a payment link and mark transaction(s) cancelled
  public async Task<IServiceResult<bool>> CancelPayOsPaymentLinkAsync(int orderCode)
  {

    try
    {
      var info = await _payOsClient.cancelPaymentLink(orderCode);

      var entity = await _unitOfWork.TransactionRepository.GetByPaymentIdAsync(orderCode);
      if (entity != null)
      {
        entity.Status = TransactionStatus.CANCELLED.ToString();
        await _unitOfWork.TransactionRepository.UpdateAsync(entity);
        await _unitOfWork.SaveChangesAsync();
      }

      return ServiceResult<bool>.Success(true);
    }
    catch (Exception ex)
    {
      return ServiceResult<bool>.Failure($"Cancel payment link failed: {ex.Message}");
    }
  }

  // Handle PayOS webhook and publish an event instead of updating directly
  public async Task<IServiceResult<bool>> HandlePayOsWebhookAsync(WebhookType webhook)
  {
    try
    {
      // Verify webhook payload (adapter may validate signature)
      var webhookData = _payOsClient.verifyPaymentWebhookData(webhook);
      Console.WriteLine($"Webhook data: {webhookData}");
      if (webhookData == null)
        return ServiceResult<bool>.Failure("Invalid webhook data");

      // Use the standardized code in webhookData to determine success
      // According to PayOS, code == "00" indicates success
      var code = webhookData.code;
      bool isSuccess = string.Equals(code, "00", StringComparison.OrdinalIgnoreCase);

      // Extract order/payment id
      var orderCodeLong = webhookData.orderCode;
      int paymentId = (int)orderCodeLong;

      // Publish an event instead of updating repository directly
      var evt = new PayOsPaymentResultEvent
      {
        PaymentId = paymentId,
        IsSuccess = isSuccess,
        Amount = webhookData.amount,
        Reference = webhookData.reference
      };

      await _eventPublisher.PublishAsync(evt);

      return ServiceResult<bool>.Success(true);
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Error handling webhook: {ex}");
      return ServiceResult<bool>.Failure($"Webhook handling failed: {ex.Message}");
    }
  }
}
