using AutoMapper;
using EV_SCMMS.Core.Domain.Models;
using EV_SCMMS.Core.Application.Results;
using EV_SCMMS.Core.Application.DTOs.MaintenanceHistoryDungVmDto;
using EV_SCMMS.Core.Application.Interfaces;
using EV_SCMMS.Core.Application.Interfaces.Services;

namespace EV_SCMMS.Infrastructure.Services
{
    public class MaintenanceHistoryDungVmService : IMaintenanceHistoryDungVmService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public MaintenanceHistoryDungVmService(IUnitOfWork unitofwork, IMapper mapper)
        {
            _unitOfWork = unitofwork;
            _mapper = mapper;
        }

        public async Task<IServiceResult<MaintenanceHistoryDungVmDto>> CreateAsync(MaintenanceHistoryDungVmCreateDto maintenanceHistoryDto)
        {
            try
            {
                var toCreateHistory = _mapper.Map<MaintenanceHistoryDungVm>(maintenanceHistoryDto);
                var result = await _unitOfWork.MaintenanceHistoryDungVmRepository.AddAsync(toCreateHistory);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResult<MaintenanceHistoryDungVmDto>.Success(_mapper.Map<MaintenanceHistoryDungVmDto>(result), "Maintenance History profile created successfully");
            }
            catch (Exception ex)
            {
               return ServiceResult<MaintenanceHistoryDungVmDto>.Failure($"Error creating Maintenance History profile: {ex.Message}");
            }
        }
        public async Task<IServiceResult<bool>> DeleteAsync(Guid maintenanceHistoryId)
        {
            try
            {
                var existingHistory = await _unitOfWork.MaintenanceHistoryDungVmRepository.GetByIdAsync(maintenanceHistoryId);
                if (existingHistory == null)
                {
                    return ServiceResult<bool>.Failure("Maintenance History profile not found.");
                }
                await _unitOfWork.MaintenanceHistoryDungVmRepository.DeleteAsync(existingHistory);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResult<bool>.Success(true, "Maintenance History profile deleted successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Failure($"Error deleteing Maintenance History: {ex.Message}");
            }
        }
        public async Task<IServiceResult<List<MaintenanceHistoryDungVmDto>>> GetAllAsync()
        {
            try
            {
                var histories =  _unitOfWork.MaintenanceHistoryDungVmRepository.GetAllAsync().Result;
                if (histories == null || !histories.Any())
                {
                    return ServiceResult<List<MaintenanceHistoryDungVmDto>>.Failure("No Maintenance Histories found.");
                }
                var historyDtos = _mapper.Map<List<MaintenanceHistoryDungVmDto>>(histories);
                return ServiceResult<List<MaintenanceHistoryDungVmDto>>.Success(historyDtos, "Maintenance Histories retrieved successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult<List<MaintenanceHistoryDungVmDto>>.Failure($": {ex.Message}");
            }
        }

        public async Task<IServiceResult<MaintenanceHistoryDungVmDto>> GetByIdAsync(Guid maintenanceHistoryId)
        {
            try
            {
                var history = await _unitOfWork.MaintenanceHistoryDungVmRepository.GetByIdAsync(maintenanceHistoryId);
                if (history == null)
                {
                    return ServiceResult<MaintenanceHistoryDungVmDto>.Failure("Maintenance History not found.");
                }
                var historyDto = _mapper.Map<MaintenanceHistoryDungVmDto>(history);
                return ServiceResult<MaintenanceHistoryDungVmDto>.Success(historyDto, "Maintenance History retrieved successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult<MaintenanceHistoryDungVmDto>.Failure($": {ex.Message}");
            }
        }

        public async Task<IServiceResult<List<MaintenanceHistoryDungVmDto>>> GetByVehicleIdAsync(Guid vehicleId)
        {
            try
            {
               var histories =  _unitOfWork.MaintenanceHistoryDungVmRepository.GetAllAsync().Result.Where(h => h.VehicleId == vehicleId).ToList();
                if (histories == null || !histories.Any())
                {
                    return ServiceResult<List<MaintenanceHistoryDungVmDto>>.Failure("No Maintenance Histories found for the vehicle.");
                }
                var historyDtos = _mapper.Map<List<MaintenanceHistoryDungVmDto>>(histories);
                return ServiceResult<List<MaintenanceHistoryDungVmDto>>.Success(historyDtos, "Maintenance Histories for the vehicle retrieved successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult<List<MaintenanceHistoryDungVmDto>>.Failure($": {ex.Message}");
            }
        }


        public async Task<IServiceResult<MaintenanceHistoryDungVmDto>> UpdateAsync(Guid maintenanceHistoryId, MaintenanceHistoryDungVmUpdateDto maintenanceHistoryDto)
        {
            try
            {
                var existingHistory = await _unitOfWork.MaintenanceHistoryDungVmRepository.GetByIdAsync(maintenanceHistoryId);
                if (existingHistory == null)
                {
                    return ServiceResult<MaintenanceHistoryDungVmDto>.Failure("Maintenance History profile not found.");
                }
                _mapper.Map(maintenanceHistoryDto, existingHistory);
                await _unitOfWork.MaintenanceHistoryDungVmRepository.UpdateAsync(existingHistory);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResult<MaintenanceHistoryDungVmDto>.Success(_mapper.Map<MaintenanceHistoryDungVmDto>(existingHistory), "Maintenance History profile updated successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult<MaintenanceHistoryDungVmDto>.Failure($"Error updating Maintenance History profile: {ex.Message}");
            }
        }
    }
}
