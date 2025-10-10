using EV_SCMMS.Core.Application.DTOs.SparepartUsageHistory;
using EV_SCMMS.Core.Application.Interfaces;
using EV_SCMMS.Core.Application.Interfaces.Services;
using EV_SCMMS.Core.Application.Results;
using EV_SCMMS.Infrastructure.Mappings;

namespace EV_SCMMS.Infrastructure.Services;

/// <summary>
/// SparepartUsageHistory service implementation for business logic operations
/// </summary>
public class SparepartUsageHistoryService : ISparepartUsageHistoryService
{
    private readonly IUnitOfWork _unitOfWork;

    public SparepartUsageHistoryService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IServiceResult<List<SparepartUsageHistoryDto>>> GetAllAsync()
    {
        try
        {
            var allResults = await _unitOfWork.SparepartUsageHistoryRepository.GetAllAsync();

            if (allResults == null || !allResults.Any())
            {
                return ServiceResult<List<SparepartUsageHistoryDto>>.Failure("No usage histories found");
            }
            
            var usageHistoryDtos = allResults.ToDto();

            return ServiceResult<List<SparepartUsageHistoryDto>>.Success(usageHistoryDtos, "Usage histories retrieved successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<List<SparepartUsageHistoryDto>>.Failure($"Error retrieving usage histories: {ex.Message}");
        }
    }

    public async Task<IServiceResult<SparepartUsageHistoryDto>> GetByIdAsync(Guid id)
    {
        try
        {
            var usageHistory = await _unitOfWork.SparepartUsageHistoryRepository.GetByIdAsync(id);
            if (usageHistory == null)
            {
                return ServiceResult<SparepartUsageHistoryDto>.Failure("Usage history not found");
            }

            var usageHistoryDto = usageHistory.ToDto();
            return ServiceResult<SparepartUsageHistoryDto>.Success(usageHistoryDto, "Usage history retrieved successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<SparepartUsageHistoryDto>.Failure($"Error retrieving usage history: {ex.Message}");
        }
    }

    public async Task<IServiceResult<SparepartUsageHistoryDto>> CreateAsync(CreateSparepartUsageHistoryDto createDto)
    {
        try
        {
            // Check if sparepart exists
            var sparepart = await _unitOfWork.SparepartRepository.GetByIdAsync(createDto.SparepartId);
            if (sparepart == null)
            {
                return ServiceResult<SparepartUsageHistoryDto>.Failure("Sparepart not found");
            }

            // Check if center exists
            var center = await _unitOfWork.CenterRepository.GetByIdAsync(createDto.CenterId);
            if (center == null)
            {
                return ServiceResult<SparepartUsageHistoryDto>.Failure("Center not found");
            }

            var usageHistory = createDto.ToEntity();
            usageHistory.Usageid = Guid.NewGuid();
            usageHistory.Createdat = DateTime.UtcNow;
            usageHistory.Isactive = true;

            var createdUsageHistory = await _unitOfWork.SparepartUsageHistoryRepository.AddAsync(usageHistory);
            await _unitOfWork.SaveChangesAsync();

            var usageHistoryDto = createdUsageHistory.ToDto();
            return ServiceResult<SparepartUsageHistoryDto>.Success(usageHistoryDto, "Usage history created successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<SparepartUsageHistoryDto>.Failure($"Error creating usage history: {ex.Message}");
        }
    }

    public async Task<IServiceResult<SparepartUsageHistoryDto>> UpdateAsync(Guid id, UpdateSparepartUsageHistoryDto updateDto)
    {
        try
        {
            var usageHistory = await _unitOfWork.SparepartUsageHistoryRepository.GetByIdAsync(id);
            if (usageHistory == null)
            {
                return ServiceResult<SparepartUsageHistoryDto>.Failure("Usage history not found");
            }

            // Check if sparepart exists
            var sparepart = await _unitOfWork.SparepartRepository.GetByIdAsync(updateDto.SparepartId);
            if (sparepart == null)
            {
                return ServiceResult<SparepartUsageHistoryDto>.Failure("Sparepart not found");
            }

            // Check if center exists
            var center = await _unitOfWork.CenterRepository.GetByIdAsync(updateDto.CenterId);
            if (center == null)
            {
                return ServiceResult<SparepartUsageHistoryDto>.Failure("Center not found");
            }

            usageHistory.UpdateFromDto(updateDto);
            usageHistory.Updatedat = DateTime.UtcNow;

            await _unitOfWork.SparepartUsageHistoryRepository.UpdateAsync(usageHistory);
            await _unitOfWork.SaveChangesAsync();

            var usageHistoryDto = usageHistory.ToDto();
            return ServiceResult<SparepartUsageHistoryDto>.Success(usageHistoryDto, "Usage history updated successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<SparepartUsageHistoryDto>.Failure($"Error updating usage history: {ex.Message}");
        }
    }

    public async Task<IServiceResult<bool>> DeleteAsync(Guid id)
    {
        try
        {
            var usageHistory = await _unitOfWork.SparepartUsageHistoryRepository.GetByIdAsync(id);
            if (usageHistory == null)
            {
                return ServiceResult<bool>.Failure("Usage history not found");
            }

            await _unitOfWork.SparepartUsageHistoryRepository.SoftDeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<bool>.Success(true, "Usage history deleted successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<bool>.Failure($"Error deleting usage history: {ex.Message}");
        }
    }

    public async Task<IServiceResult<List<SparepartUsageHistoryDto>>> GetBySparepartIdAsync(Guid sparepartId)
    {
        try
        {
            var usageHistories = await _unitOfWork.SparepartUsageHistoryRepository.GetBySparepartIdAsync(sparepartId);
            var usageHistoryDtos = usageHistories.ToDto();

            return ServiceResult<List<SparepartUsageHistoryDto>>.Success(usageHistoryDtos, "Usage histories retrieved successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<List<SparepartUsageHistoryDto>>.Failure($"Error retrieving usage histories by sparepart: {ex.Message}");
        }
    }

    public async Task<IServiceResult<List<SparepartUsageHistoryDto>>> GetByCenterIdAsync(Guid centerId)
    {
        try
        {
            var usageHistories = await _unitOfWork.SparepartUsageHistoryRepository.GetByCenterIdAsync(centerId);
            var usageHistoryDtos = usageHistories.ToDto();

            return ServiceResult<List<SparepartUsageHistoryDto>>.Success(usageHistoryDtos, "Usage histories retrieved successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<List<SparepartUsageHistoryDto>>.Failure($"Error retrieving usage histories by center: {ex.Message}");
        }
    }

    public async Task<IServiceResult<List<SparepartUsageHistoryDto>>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        try
        {
            var usageHistories = await _unitOfWork.SparepartUsageHistoryRepository.GetByDateRangeAsync(startDate, endDate);
            var usageHistoryDtos = usageHistories.ToDto();

            return ServiceResult<List<SparepartUsageHistoryDto>>.Success(usageHistoryDtos, "Usage histories retrieved successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<List<SparepartUsageHistoryDto>>.Failure($"Error retrieving usage histories by date range: {ex.Message}");
        }
    }

    public async Task<IServiceResult<List<SparepartUsageHistoryDto>>> GetByVehicleIdAsync(string vehicleId)
    {
        try
        {
            var usageHistories = await _unitOfWork.SparepartUsageHistoryRepository.GetByVehicleIdAsync(vehicleId);
            var usageHistoryDtos = usageHistories.ToDto();

            return ServiceResult<List<SparepartUsageHistoryDto>>.Success(usageHistoryDtos, "Usage histories retrieved successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<List<SparepartUsageHistoryDto>>.Failure($"Error retrieving usage histories by vehicle: {ex.Message}");
        }
    }

    public Task<IServiceResult<object>> GetUsageStatisticsAsync(Guid sparepartId, DateTime startDate, DateTime endDate)
    {
        throw new NotImplementedException();
    }
}