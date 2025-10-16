using AutoMapper;
using EV_SCMMS.Core.Application.DTOs.MaintenanceTaskDungVmDto;
using EV_SCMMS.Core.Application.Interfaces;
using EV_SCMMS.Core.Application.Interfaces.Services;
using EV_SCMMS.Core.Application.Results;
using EV_SCMMS.Core.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_SCMMS.Infrastructure.Services
{
    public class MaintenanceTaskDungVmService : IMaintenanceTaskDungVmService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public MaintenanceTaskDungVmService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<IServiceResult<MaintenanceTaskDungVmDto>> CreateAsync(MaintenanceTaskDungVmCreateDto maintenanceTaskDto)
        {
            try
            {
                var orderService = await _unitOfWork.OrderServiceThaoNttRepository.GetByIdAsync(maintenanceTaskDto.OrderServiceId);
                if (orderService == null)
                {
                    return ServiceResult<MaintenanceTaskDungVmDto>.Failure("Order Service not found.");
                }

                var toCreateTask = _mapper.Map<MaintenanceTaskDungVm>(maintenanceTaskDto);
                var result = await _unitOfWork.MaintenanceTaskDungVmRepository.AddAsync(toCreateTask);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResult<MaintenanceTaskDungVmDto>.Success(_mapper.Map<MaintenanceTaskDungVmDto>(result), "Maintenance Task craeted successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult<MaintenanceTaskDungVmDto>.Failure($"Error creating Maintenance Task: {ex.Message}");
            }
        }


        public async Task<IServiceResult<MaintenanceTaskDungVmDto>> UpdateAsync
            (Guid maintenanceTaskId, MaintenanceTaskDungVmUpdateDto maintenanceTaskDto)
        {       
            try
            {
                var existingTask = await _unitOfWork.MaintenanceTaskDungVmRepository.GetByIdAsync(maintenanceTaskId);
                if (existingTask == null)
                {
                    return ServiceResult<MaintenanceTaskDungVmDto>.Failure("Maintenance Task not found.");
                }
                if (maintenanceTaskDto.TechnicianId != null)
                {
                    var technician = await _unitOfWork.UserRepository.GetByIdAsync(maintenanceTaskDto.TechnicianId.Value);
                    if (technician == null)
                    {
                        return ServiceResult<MaintenanceTaskDungVmDto>.Failure("Technician not found.");
                    }
                }

                existingTask.UpdatedAt = DateTime.UtcNow;
                _mapper.Map(maintenanceTaskDto, existingTask);

                await _unitOfWork.MaintenanceTaskDungVmRepository.UpdateAsync(existingTask);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResult<MaintenanceTaskDungVmDto>.Success(_mapper.Map<MaintenanceTaskDungVmDto>(existingTask), "Maintenance Task updated successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult<MaintenanceTaskDungVmDto>.Failure($"Error updating Maintenance Task: {ex.Message}");
            }
        }

        public async Task<IServiceResult<bool>> DeleteAsync(Guid maintenanceTaskId)
        {
            try
            {
                var existingTask = await _unitOfWork.MaintenanceTaskDungVmRepository.GetByIdAsync(maintenanceTaskId);
                if (existingTask == null)
                {
                    return ServiceResult<bool>.Failure("Maintenance Task not found.");
                }

                await _unitOfWork.MaintenanceTaskDungVmRepository.DeleteAsync(existingTask);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResult<bool>.Success(true, "Maintenance Task deleted successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Failure($"Error deleting Maintenance Task: {ex.Message}");
            }
        }

        public async Task<IServiceResult<List<MaintenanceTaskDungVmDto>>> GetAllAsync()
        {
            try
            {
                var tasks = await _unitOfWork.MaintenanceTaskDungVmRepository.GetAllAsync();
                if (tasks == null || !tasks.Any())
                {
                    return ServiceResult<List<MaintenanceTaskDungVmDto>>.Failure("No Maintenance Tasks found.");
                }

                var taskDtos = _mapper.Map<List<MaintenanceTaskDungVmDto>>(tasks);
                return ServiceResult<List<MaintenanceTaskDungVmDto>>.Success(taskDtos, "Maintenance Tasks retrieved successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult<List<MaintenanceTaskDungVmDto>>.Failure($"Error retrieving Maintenance Tasks: {ex.Message}");
            }
        }

        public async Task<IServiceResult<MaintenanceTaskDungVmDto>> GetByIdAsync(Guid maintenanceTaskId)
        {
            try
            {
                var task = await _unitOfWork.MaintenanceTaskDungVmRepository.GetByIdAsync(maintenanceTaskId);
                if (task == null)
                {
                    return ServiceResult<MaintenanceTaskDungVmDto>.Failure("Maintenance Task not found.");
                }
                var taskDto = _mapper.Map<MaintenanceTaskDungVmDto>(task);
                return ServiceResult<MaintenanceTaskDungVmDto>.Success(taskDto, "Maintenance Task retrieved successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult<MaintenanceTaskDungVmDto>.Failure($"Error retrieving Maintenance Task: {ex.Message}");
            }
        }

        public async Task<IServiceResult<List<MaintenanceTaskDungVmDto>>> GetByOrderServiceIdAsync(int orderServiceId)
        {
            try
            {
                var orderServices = await _unitOfWork.MaintenanceTaskDungVmRepository.GetByOrderServiceIdAsync(orderServiceId);
                if (orderServices == null || !orderServices.Any())
                {
                    return ServiceResult<List<MaintenanceTaskDungVmDto>>.Failure("No Order Services found for the given Order ID.");
                }

                var orderServiceDtos = _mapper.Map<List<MaintenanceTaskDungVmDto>>(orderServices);
                return ServiceResult<List<MaintenanceTaskDungVmDto>>.Success(orderServiceDtos, "Order Services retrieved successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult<List<MaintenanceTaskDungVmDto>>.Failure($"Error retrieving Order Services: {ex.Message}");
            }
        }

        


    }
}
