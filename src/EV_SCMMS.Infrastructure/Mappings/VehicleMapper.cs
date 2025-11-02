using EV_SCMMS.Core.Application.DTOs.User;
using EV_SCMMS.Core.Application.DTOs.Vehicle;
using EV_SCMMS.Core.Domain.Models;

namespace EV_SCMMS.Infrastructure.Mappings;

/// <summary>
/// Extension methods for Vehicle entity mapping
/// </summary>
public static class VehicleMapperExtensions
{
  /// <summary>
  /// Map Vehicle entity to VehicleDto
  /// </summary>
  /// <param name="vehicle">Vehicle entity</param>
  /// <returns>VehicleDto</returns>
  public static VehicleDto ToDto(this Vehicle vehicle)
  {
    if (vehicle == null) return null;

    return new VehicleDto
    {
      VehicleId = vehicle.Vehicleid,
      CustomerId = vehicle.Customerid,
      ModelId = vehicle.Modelid ?? 0,
      LicensePlate = vehicle.Licenseplate ?? string.Empty,
      Year = vehicle.Year,
      Color = vehicle.Color,
      Status = vehicle.Status,
      IsActive = vehicle.Isactive ?? false,
      CreatedAt = vehicle.Createdat,
      UpdatedAt = vehicle.Updatedat
    };
  }

  /// <summary>
  /// Map list of Vehicle entities to list of VehicleDto
  /// </summary>
  /// <param name="vehicles">List of Vehicle entities</param>
  /// <returns>List of VehicleDto</returns>
  public static List<VehicleDto> ToDto(this IEnumerable<Vehicle> vehicles)
  {
    if (vehicles == null) return new List<VehicleDto>();

    return vehicles.Select(v => v.ToDto()).Where(dto => dto != null).ToList();
  }

  /// <summary>
  /// Map CreateVehicleDto to Vehicle entity
  /// </summary>
  /// <param name="createDto">CreateVehicleDto</param>
  /// <returns>Vehicle entity</returns>
  public static Vehicle ToEntity(this CreateVehicleDto createDto, Guid customerId)
  {
    if (createDto == null) return null;

    return new Vehicle
    {
      Vehicleid = Guid.NewGuid(),
      Customerid = customerId,  // Sẽ set từ JWT token trong Controller/Service
      Modelid = createDto.ModelId,
      Licenseplate = createDto.LicensePlate ?? string.Empty,
      Year = createDto.Year,
      Color = createDto.Color,
      Status = createDto.Status ?? "ACTIVE",  // Default nếu null
      Isactive = createDto.IsActive ?? true,
      Createdat = DateTime.UtcNow,
      Updatedat = null
    };
  }

  /// <summary>
  /// Update existing Vehicle entity with UpdateVehicleDto data
  /// </summary>
  /// <param name="entity">Existing Vehicle entity</param>
  /// <param name="updateDto">UpdateVehicleDto</param>
  public static void UpdateFromDto(this Vehicle entity, UpdateVehicleDto updateDto)
  {
    if (entity == null || updateDto == null) return;

    entity.Modelid = updateDto.ModelId;
    entity.Licenseplate = updateDto.LicensePlate;
    entity.Year = updateDto.Year;
    entity.Color = updateDto.Color;
    entity.Status = updateDto.Status;
    entity.Isactive = updateDto.IsActive;
    entity.Updatedat = DateTime.UtcNow;
  }

  public static VehicleDetailDto ToDetailDto(this Vehicle vehicle)
  {
    if (vehicle == null) return null;

    return new VehicleDetailDto
    {
      VehicleId = vehicle.Vehicleid,
      CustomerId = vehicle.Customerid,
      ModelId = vehicle.Modelid ?? 0,
      LicensePlate = vehicle.Licenseplate ?? string.Empty,
      Year = vehicle.Year,
      Color = vehicle.Color,
      Status = vehicle.Status,
      IsActive = vehicle.Isactive ?? false,
      CreatedAt = vehicle.Createdat,
      UpdatedAt = vehicle.Updatedat,
      Customer = vehicle.Customer?.ToDto() ?? new UserDto()  // Map Customer nếu có
    };
  }
}
