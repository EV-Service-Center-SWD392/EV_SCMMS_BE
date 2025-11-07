using EV_SCMMS.Core.Application.DTOs.SparepartForecast;
using EV_SCMMS.Core.Application.Interfaces;
using EV_SCMMS.Core.Application.Interfaces.Services;
using EV_SCMMS.Core.Application.Results;
using EV_SCMMS.Infrastructure.Mappings;

namespace EV_SCMMS.Infrastructure.Services;

/// <summary>
/// SparepartForecast service implementation for business logic operations
/// </summary>
public class SparepartForecastService : ISparepartForecastService
{
    private readonly IUnitOfWork _unitOfWork;

    public SparepartForecastService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IServiceResult<List<SparepartForecastDto>>> GetAllAsync()
    {
        try
        {
            var allResults = await _unitOfWork.SparepartForecastRepository.GetAllAsync();

            if (allResults == null || !allResults.Any())
            {
                return ServiceResult<List<SparepartForecastDto>>.Failure("No sparepart forecasts found");
            }
            
            var forecastDtos = allResults.ToDto();

            if (forecastDtos == null || !forecastDtos.Any())
            {
                return ServiceResult<List<SparepartForecastDto>>.Failure("No sparepart forecasts found");
            }

            return ServiceResult<List<SparepartForecastDto>>.Success(forecastDtos, "Sparepart forecasts retrieved successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<List<SparepartForecastDto>>.Failure($"Error retrieving sparepart forecasts: {ex.Message}");
        }
    }

    public async Task<IServiceResult<SparepartForecastDto>> GetByIdAsync(Guid id)
    {
        try
        {
            var forecast = await _unitOfWork.SparepartForecastRepository.GetByIdAsync(id);
            if (forecast == null)
            {
                return ServiceResult<SparepartForecastDto>.Failure("Sparepart forecast not found");
            }

            var forecastDto = forecast.ToDto();
            return ServiceResult<SparepartForecastDto>.Success(forecastDto, "Sparepart forecast retrieved successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<SparepartForecastDto>.Failure($"Error retrieving sparepart forecast: {ex.Message}");
        }
    }

    public async Task<IServiceResult<SparepartForecastDto>> CreateAsync(CreateSparepartForecastDto createDto)
    {
        try
        {
            // Check if sparepart exists
            var sparepart = await _unitOfWork.SparepartRepository.GetByIdAsync(createDto.SparepartId);
            if (sparepart == null)
            {
                return ServiceResult<SparepartForecastDto>.Failure("Sparepart not found");
            }

            // Check if center exists
            var center = await _unitOfWork.CenterRepository.GetByIdAsync(createDto.CenterId);
            if (center == null)
            {
                return ServiceResult<SparepartForecastDto>.Failure("Center not found");
            }

            var forecast = createDto.ToEntity();
            forecast.Forecastid = Guid.NewGuid();
            forecast.Createdat = DateTime.UtcNow;
            forecast.Isactive = true;

            var createdForecast = await _unitOfWork.SparepartForecastRepository.AddAsync(forecast);

            var forecastDto = createdForecast.ToDto();
            return ServiceResult<SparepartForecastDto>.Success(forecastDto, "Sparepart forecast created successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<SparepartForecastDto>.Failure($"Error creating sparepart forecast: {ex.Message}");
        }
    }

    public async Task<IServiceResult<SparepartForecastDto>> UpdateAsync(Guid id, UpdateSparepartForecastDto updateDto)
    {
        try
        {
            var forecast = await _unitOfWork.SparepartForecastRepository.GetByIdAsync(id);
            if (forecast == null)
            {
                return ServiceResult<SparepartForecastDto>.Failure("Sparepart forecast not found");
            }

            // Check if sparepart exists
            var sparepart = await _unitOfWork.SparepartRepository.GetByIdAsync(updateDto.SparepartId);
            if (sparepart == null)
            {
                return ServiceResult<SparepartForecastDto>.Failure("Sparepart not found");
            }

            // Check if center exists
            var center = await _unitOfWork.CenterRepository.GetByIdAsync(updateDto.CenterId);
            if (center == null)
            {
                return ServiceResult<SparepartForecastDto>.Failure("Center not found");
            }

            forecast.UpdateFromDto(updateDto);
            forecast.Updatedat = DateTime.UtcNow;

            await _unitOfWork.SparepartForecastRepository.UpdateAsync(forecast);

            var forecastDto = forecast.ToDto();
            return ServiceResult<SparepartForecastDto>.Success(forecastDto, "Sparepart forecast updated successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<SparepartForecastDto>.Failure($"Error updating sparepart forecast: {ex.Message}");
        }
    }

    public async Task<IServiceResult<bool>> DeleteAsync(Guid id)
    {
        try
        {
            var forecast = await _unitOfWork.SparepartForecastRepository.GetByIdAsync(id);
            if (forecast == null)
            {
                return ServiceResult<bool>.Failure("Sparepart forecast not found");
            }

            await _unitOfWork.SparepartForecastRepository.SoftDeleteAsync(id);

            return ServiceResult<bool>.Success(true, "Sparepart forecast deleted successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<bool>.Failure($"Error deleting sparepart forecast: {ex.Message}");
        }
    }

    public async Task<IServiceResult<List<SparepartForecastDto>>> GetBySparepartIdAsync(Guid sparepartId)
    {
        try
        {
            var forecasts = await _unitOfWork.SparepartForecastRepository.GetBySparepartIdAsync(sparepartId);
            var forecastDtos = forecasts.ToDto();

            return ServiceResult<List<SparepartForecastDto>>.Success(forecastDtos, "Sparepart forecasts retrieved successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<List<SparepartForecastDto>>.Failure($"Error retrieving forecasts by sparepart: {ex.Message}");
        }
    }

    public async Task<IServiceResult<List<SparepartForecastDto>>> GetByCenterIdAsync(Guid centerId)
    {
        try
        {
            var forecasts = await _unitOfWork.SparepartForecastRepository.GetByCenterIdAsync(centerId);
            var forecastDtos = forecasts.ToDto();

            return ServiceResult<List<SparepartForecastDto>>.Success(forecastDtos, "Sparepart forecasts retrieved successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<List<SparepartForecastDto>>.Failure($"Error retrieving forecasts by center: {ex.Message}");
        }
    }

    public async Task<IServiceResult<List<SparepartForecastDto>>> GetLowReorderPointForecastsAsync()
    {
        try
        {
            var forecasts = await _unitOfWork.SparepartForecastRepository.GetLowReorderPointForecastsAsync();
            var forecastDtos = forecasts.ToDto();

            return ServiceResult<List<SparepartForecastDto>>.Success(forecastDtos, "Low reorder point forecasts retrieved successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<List<SparepartForecastDto>>.Failure($"Error retrieving low reorder point forecasts: {ex.Message}");
        }
    }

    public async Task<IServiceResult<SparepartForecastDto>> ApproveForecastAsync(Guid id, string approvedBy)
    {
        try
        {
            var forecast = await _unitOfWork.SparepartForecastRepository.GetByIdAsync(id);
            if (forecast == null)
            {
                return ServiceResult<SparepartForecastDto>.Failure("Sparepart forecast not found");
            }

            // Note: ApprovedBy field appears to be Guid in DTO but string parameter
            // You might need to adjust this based on your actual data structure
            forecast.Approveddate = DateTime.UtcNow;
            forecast.Status = "APPROVED";
            forecast.Updatedat = DateTime.UtcNow;

            await _unitOfWork.SparepartForecastRepository.UpdateAsync(forecast);

            var forecastDto = forecast.ToDto();
            return ServiceResult<SparepartForecastDto>.Success(forecastDto, "Sparepart forecast approved successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<SparepartForecastDto>.Failure($"Error approving sparepart forecast: {ex.Message}");
        }
    }
}