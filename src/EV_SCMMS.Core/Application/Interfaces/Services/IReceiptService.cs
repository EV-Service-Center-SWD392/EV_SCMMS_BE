using EV_SCMMS.Core.Application.Results;
using EV_SCMMS.Core.Application.DTOs.Receipt;

namespace EV_SCMMS.Core.Application.Interfaces.Services;

public interface IReceiptService
{
    Task<IServiceResult<List<ReceiptDto>>> GetAllAsync();
    Task<IServiceResult<ReceiptDto?>> GetByIdAsync(Guid id);
    Task<IServiceResult<List<ReceiptDto>>> GetByCustomerIdAsync(Guid customerId);
    Task<IServiceResult<List<ReceiptDto>>> GetForCurrentUserAsync();
}
