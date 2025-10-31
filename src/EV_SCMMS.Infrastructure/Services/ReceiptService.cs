using EV_SCMMS.Core.Application.Interfaces.Services;
using EV_SCMMS.Core.Application.Interfaces;
using EV_SCMMS.Core.Domain.Models;
using Microsoft.EntityFrameworkCore;
using EV_SCMMS.Core.Application.Results;
using EV_SCMMS.Core.Application.DTOs.Receipt;
using EV_SCMMS.Infrastructure.Mappings;
using EV_SCMMS.Core.Application.Interfaces.Services;

namespace EV_SCMMS.Infrastructure.Services;

public class ReceiptService : IReceiptService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public ReceiptService(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<ServiceResult<List<ReceiptDto>>> GetAllAsync()
    {
        var receipts = await _unitOfWork.ReceiptRepository.GetAllAsync();

        var dtos = receipts.Select(r => r.ToDto()).ToList();
        return ServiceResult<List<ReceiptDto>>.Success(dtos);
    }

    public async Task<ServiceResult<ReceiptDto?>> GetByIdAsync(Guid id)
    {
        var r = await _unitOfWork.ReceiptRepository.GetByIdWithItemsAsync(id);
    if(_currentUser.Role == "CUSTOMER" && _currentUser.UserId.HasValue && _currentUser.UserId.Value != r.Customerid)
        return ServiceResult<ReceiptDto?>.Failure("Access denied");
    if (r == null) return ServiceResult<ReceiptDto?>.Failure("Receipt not found");

        return ServiceResult<ReceiptDto?>.Success(r.ToDto());
    }

    public async Task<ServiceResult<List<ReceiptDto>>> GetByCustomerIdAsync(Guid customerId)
    {
        var receipts = await _unitOfWork.ReceiptRepository.GetByCustomerIdAsync(customerId);

        var dtos = receipts.Select(r => r.ToDto()).ToList();
        return ServiceResult<List<ReceiptDto>>.Success(dtos);
    }

    public async Task<ServiceResult<List<ReceiptDto>>> GetForCurrentUserAsync()
    {
        if (!_currentUser.IsAuthenticated || !_currentUser.UserId.HasValue)
            return ServiceResult<List<ReceiptDto>>.Failure("User not authenticated");

        var userId = _currentUser.UserId.Value;
        var receipts = await _unitOfWork.ReceiptRepository.GetByCustomerIdAsync(userId);
        var dtos = receipts.Select(r => r.ToDto()).ToList();
        return ServiceResult<List<ReceiptDto>>.Success(dtos);
    }
}
