using AutoMapper;
using EV_SCMMS.Core.Application.DTOs.VehicleConditionDungVm;
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
    public class VehicleConditionDungVmService : IVehicleConditionDungVmService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public VehicleConditionDungVmService(IUnitOfWork unitofwork, IMapper mapper)
        {
            _unitOfWork = unitofwork;
            _mapper = mapper;
        }

        public async Task<IServiceResult<VehicleConditionDungVmDto>> CreateAsync
            (VehicleConditionDungVmCreateDto vehicleConditionDto)
        {
            try
            {
                //var vehicleCondition = await _unitOfWork.VehicleRepository.GetByIdAsync(vehicleConditionDto.VehicleId);
                //if (vehicleCondition == null)
                //{
                //    return ServiceResult<VehicleConditionDungVmDto>.Failure("Vehicle not found.");
                // 
                var toCreateCondition = _mapper.Map<VehicleConditionDungVm>(vehicleConditionDto);
                var result = await _unitOfWork.VehicleConditionDungVmRepository.AddAsync(toCreateCondition);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResult<VehicleConditionDungVmDto>.Success(_mapper.Map<VehicleConditionDungVmDto>(result), "Vehicle Condition profile created successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult<VehicleConditionDungVmDto>.Failure($"Error creating Vehicle Condition profile: {ex.Message}");
            }

        }

        public async Task<IServiceResult<bool>> DeleteAsync(Guid vehicleConditionId)
        {
            try
            {
                var existingCondition = await _unitOfWork.VehicleConditionDungVmRepository.GetByIdAsync(vehicleConditionId);
                if (existingCondition == null)
                {
                    return ServiceResult<bool>.Failure("Vehicle Condition profile not found.");
                }
                await _unitOfWork.VehicleConditionDungVmRepository.DeleteAsync(existingCondition);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResult<bool>.Success(true, "Vehicle Condition profile deleted successfully");

            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Failure($"Error deleting Vehicle Condition profile: {ex.Message}");
            }
        }

        public async Task<IServiceResult<List<VehicleConditionDungVmDto>>> GetAllAsync()
        {
            try
            {
                var conditionList = await _unitOfWork.VehicleConditionDungVmRepository.GetAllAsync();
                if (conditionList == null || !conditionList.Any())
                {
                    return ServiceResult<List<VehicleConditionDungVmDto>>.Failure("No Vehicle Condition profiles found.");
                }

                var conditionDtoList = _mapper.Map<List<VehicleConditionDungVmDto>>(conditionList);
                return ServiceResult<List<VehicleConditionDungVmDto>>.Success(conditionDtoList, "Vehicle Condition profiles retrieved successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult<List<VehicleConditionDungVmDto>>.Failure($"Error retrieving Vehicle Condition profiles: {ex.Message}");
            }
        }

        public async Task<IServiceResult<VehicleConditionDungVmDto>> GetByIdAsync(Guid vehicleConditionId)
        {
            try
            {
                var condition = await _unitOfWork.VehicleConditionDungVmRepository.GetByIdAsync(vehicleConditionId);
                if (condition == null)
                {
                    return ServiceResult<VehicleConditionDungVmDto>.Failure("Vehicle Condition profile not found.");
                }
                var conditionDto = _mapper.Map<VehicleConditionDungVmDto>(condition);
                return ServiceResult<VehicleConditionDungVmDto>.Success(conditionDto, "Vehicle Condition profile retrieved successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult<VehicleConditionDungVmDto>.Failure($"Error retrieving Vehicle Condition profile: {ex.Message}");
            }
        }

        public async Task<IServiceResult<List<VehicleConditionDungVmDto>>> GetByVehicleIdAsync(Guid vehicleId)
        {
            try
            {
                var conditionList = await _unitOfWork.VehicleConditionDungVmRepository.GetByVehicleIdAsync(vehicleId);
                if (conditionList == null || !conditionList.Any())
                {
                    return ServiceResult<List<VehicleConditionDungVmDto>>.Failure("Vehicle Condition profile not found.");
                }

                var conditionDtoList = _mapper.Map<List<VehicleConditionDungVmDto>>(conditionList);
                return ServiceResult<List<VehicleConditionDungVmDto>>.Success(conditionDtoList, "Vehicle Condition profiles retrieved successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult<List<VehicleConditionDungVmDto>>.Failure($"Error retrieving Vehicle Condition profiles: {ex.Message}");
            }
        }
       
        public async Task<IServiceResult<VehicleConditionDungVmDto>> UpdateAsync(Guid vehicleConditionId, VehicleConditionDungVmUpdateDto vehicleConditionDto)
        {
            try
            {
                var existingCondition = await _unitOfWork.VehicleConditionDungVmRepository.GetByIdAsync(vehicleConditionId);
                if (existingCondition == null)
                {
                    return ServiceResult<VehicleConditionDungVmDto>.Failure("Vehicle Condition profile not found.");
                }
                _mapper.Map(vehicleConditionDto, existingCondition);
                await _unitOfWork.VehicleConditionDungVmRepository.UpdateAsync(existingCondition);
                await _unitOfWork.SaveChangesAsync();

                var updatedConditionDto = _mapper.Map<VehicleConditionDungVmDto>(existingCondition);
                return ServiceResult<VehicleConditionDungVmDto>.Success(updatedConditionDto, "Vehicle Condition profile updated successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult<VehicleConditionDungVmDto>.Failure($"Error updating Vehicle Condition profile: {ex.Message}");
            }
        }
    }
}
