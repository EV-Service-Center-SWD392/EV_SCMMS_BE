using EV_SCMMS.Core.Application.Results;
using EV_SCMMS.Core.Application.DTOs.Receipt;

namespace EV_SCMMS.Core.Application.Interfaces.Services;

public interface IReceiptService
{
    Task<ServiceResult<List<ReceiptDto>>> GetAllAsync();
    Task<ServiceResult<ReceiptDto?>> GetByIdAsync(Guid id);
    Task<ServiceResult<List<ReceiptDto>>> GetByCustomerIdAsync(Guid customerId);
    Task<ServiceResult<List<ReceiptDto>>> GetForCurrentUserAsync();
}
