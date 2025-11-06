using EV_SCMMS.Core.Application.DTOs.Common;
using EV_SCMMS.Core.Application.DTOs.Vehicle;
using EV_SCMMS.Core.Application.Interfaces;
using EV_SCMMS.Core.Application.Results;
using EV_SCMMS.Core.Domain.Models;
using EV_SCMMS.Infrastructure.Mappings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Microsoft.Extensions.Logging;

public class VehicleService : IVehicleService
{
  private readonly IUnitOfWork _unitOfWork;

  public VehicleService(IUnitOfWork unitOfWork)
  {
    _unitOfWork = unitOfWork;
  }

  public async Task<IServiceResult<PagedResult<VehicleDto>>> GetListVehiclesAsync(VehicleQueryDto query, Guid customerId)
  {
    var queryable = _unitOfWork.VehicleRepository.GetAllQueryable().AsNoTracking();

    queryable = queryable.Where(x => x.Customerid == customerId);

    if (!string.IsNullOrEmpty(query.Status))
      queryable = queryable.Where(v => v.Status.ToUpper() == query.Status.ToUpper());

    if (query.Year.HasValue)
      queryable = queryable.Where(v => v.Year == query.Year.Value);

    if (query.FromDate.HasValue)
      queryable = queryable.Where(v => v.Createdat >= query.FromDate.Value);

    if (query.ToDate.HasValue)
      queryable = queryable.Where(v => v.Createdat <= query.ToDate.Value);

    var totalCount = await queryable.CountAsync();

    var items = await queryable
        .OrderBy(v => v.Createdat)
        .Skip((query.Page - 1) * query.PageSize)
        .Take(query.PageSize)
        .ToListAsync();

    var dtos = items.ToDto();

    return ServiceResult<PagedResult<VehicleDto>>.Success(new PagedResult<VehicleDto>
    {
      Items = dtos,
      Total = totalCount,
      Page = query.Page,
      PageSize = query.PageSize
    }, "Get list successfully");
  }

  public async Task<IServiceResult<Vehicle>> CreateAsync(CreateVehicleDto dto, Guid customerId, CancellationToken ct = default)
  {
    var user = await _unitOfWork.UserAccountRepository.GetByIdAsync(customerId);
    var model = await _unitOfWork.VehicleModelRepository.GetByIdAsync(dto.ModelId);

    if (user == null)
    {
      return ServiceResult<Vehicle>.Failure("User not found");
    }

    if (model == null)
    {
      return ServiceResult<Vehicle>.Failure("Vehicle model not found");
    }

    var mappedVehicle = dto.ToEntity(customerId);

    var createdVehicle = await _unitOfWork.VehicleRepository.AddAsync(mappedVehicle);

    await _unitOfWork.SaveChangesAsync(ct);

    return ServiceResult<Vehicle>.Success(createdVehicle, "Success");
  }

  public async Task<IServiceResult<Vehicle>> DeleteAsync(Guid id, Guid customerId, CancellationToken ct = default)
  {
    var user = await _unitOfWork.UserAccountRepository.GetByIdAsync(customerId);

    var currentVehicle = await _unitOfWork.VehicleRepository.GetByIdAsync(id);

    if (user == null)
    {
      return ServiceResult<Vehicle>.Failure("User not found");
    }

    if (currentVehicle == null)
    {
      return ServiceResult<Vehicle>.Failure("Vehicle was not found");
    }


    await _unitOfWork.VehicleRepository.RemoveAsync(currentVehicle);

    await _unitOfWork.SaveChangesAsync(ct);

    return ServiceResult<Vehicle>.Success(currentVehicle, "Delete vehicle successfully");
  }

  public async Task<IServiceResult<VehicleDetailDto>> GetVehicleAsync(Guid id, Guid customerId)
  {
    try
    {
      var queryable = _unitOfWork.VehicleRepository.GetAllQueryable();

      var vehicle = await queryable
          .Include(v => v.Customer)
          .FirstOrDefaultAsync(v => v.Vehicleid == id && v.Customerid == customerId);

      if (vehicle == null)
      {
        return ServiceResult<VehicleDetailDto>.Failure("Vehicle not found");

      }

      var detailDto = vehicle.ToDetailDto();

      return ServiceResult<VehicleDetailDto>.Success(detailDto, "Get vehicle details successfully");
    }
    catch (Exception ex)
    {
      Console.WriteLine(ex);
      return ServiceResult<VehicleDetailDto>.Failure("Error retrieving vehicle");
    }
  }

  public async Task<IServiceResult<Vehicle>> UpdateAsync(Guid id, UpdateVehicleDto dto, Guid customerId, CancellationToken ct = default)
  {
    try
    {
      var user = await _unitOfWork.UserAccountRepository.GetByIdAsync(customerId);
      var model = await _unitOfWork.VehicleModelRepository.GetByIdAsync(dto.ModelId);

      var currentVehicle = await _unitOfWork.VehicleRepository.GetByIdAsync(id);

      if (user == null)
      {
        return ServiceResult<Vehicle>.Failure("User not found");
      }

      if (model == null)
      {
        return ServiceResult<Vehicle>.Failure("Assign vehicle model not found");
      }

      if (currentVehicle == null)
      {
        return ServiceResult<Vehicle>.Failure("Vehicle was not found");
      }

      currentVehicle.UpdateFromDto(dto);

      await _unitOfWork.VehicleRepository.UpdateAsync(currentVehicle);

      await _unitOfWork.VehicleRepository.SaveAsync();

      return ServiceResult<Vehicle>.Success(currentVehicle, "Update Successfully");
    }
    catch (System.Exception)
    {

      return ServiceResult<Vehicle>.Failure("Failed to create a vehicle");
    }
  }

  public async Task<IServiceResult<List<Vehiclemodel>>> GetListVehicleModel()
  {
    var vehicleModels = await _unitOfWork.VehicleModelRepository.GetAllAsync();

    return ServiceResult<List<Vehiclemodel>>.Success(vehicleModels, "Get list successfully");
  }
}
