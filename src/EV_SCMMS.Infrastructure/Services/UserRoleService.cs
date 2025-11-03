using EV_SCMMS.Core.Application.DTOs.UserRole;
using EV_SCMMS.Core.Application.Interfaces;
using EV_SCMMS.Core.Application.Interfaces.Services;
using EV_SCMMS.Core.Application.Results;
using EV_SCMMS.Infrastructure.Mappings;

namespace EV_SCMMS.Infrastructure.Services;

public class UserRoleService : IUserRoleService
{
    private readonly IUnitOfWork _unitOfWork;

    public UserRoleService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IServiceResult<UserRoleDto>> CreateAsync(CreateUserRoleDto dto)
    {
        try
        {
            var existing = await _unitOfWork.UserRoleRepository.GetByNameAsync(dto.Name);
            if (existing != null)
            {
                return ServiceResult<UserRoleDto>.Failure("Role name already exists");
            }

            var entity = dto.ToEntity();
            await _unitOfWork.UserRoleRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<UserRoleDto>.Success(entity.ToDto(), "User role created successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<UserRoleDto>.Failure($"Error creating user role: {ex.Message}");
        }
    }

    public async Task<IServiceResult<UserRoleDto>> UpdateAsync(Guid id, UpdateRoleDto dto)
    {
        try
        {
            var existing = await _unitOfWork.UserRoleRepository.GetByIdAsync(id);
            if (existing == null)
            {
                return ServiceResult<UserRoleDto>.Failure("User role not found");
            }

            existing.UpdateFromDto(dto);
            await _unitOfWork.UserRoleRepository.UpdateAsync(existing);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<UserRoleDto>.Success(existing.ToDto(), "User role updated successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<UserRoleDto>.Failure($"Error updating user role: {ex.Message}");
        }
    }

    public async Task<IServiceResult<bool>> DeleteAsync(Guid id)
    {
        try
        {
            var existing = await _unitOfWork.UserRoleRepository.GetByIdAsync(id);
            if (existing == null)
            {
                return ServiceResult<bool>.Failure("User role not found");
            }

            existing.Isactive = false;
            existing.Updatedat = DateTime.UtcNow;
            await _unitOfWork.UserRoleRepository.UpdateAsync(existing);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<bool>.Success(true, "User role deleted successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<bool>.Failure($"Error deleting user role: {ex.Message}");
        }
    }

    public async Task<IServiceResult<UserRoleDto>> GetByIdAsync(Guid id)
    {
        try
        {
            var entity = await _unitOfWork.UserRoleRepository.GetByIdAsync(id);
            if (entity == null)
            {
                return ServiceResult<UserRoleDto>.Failure("User role not found");
            }

            return ServiceResult<UserRoleDto>.Success(entity.ToDto());
        }
        catch (Exception ex)
        {
            return ServiceResult<UserRoleDto>.Failure($"Error retrieving user role: {ex.Message}");
        }
    }

    public async Task<IServiceResult<List<UserRoleDto>>> GetAllAsync()
    {
        try
        {
            var items = await _unitOfWork.UserRoleRepository.GetAllAsync();
            return ServiceResult<List<UserRoleDto>>.Success(items.Select(x => x.ToDto()).Where(x => x != null).ToList());
        }
        catch (Exception ex)
        {
            return ServiceResult<List<UserRoleDto>>.Failure($"Error retrieving user roles: {ex.Message}");
        }
    }

    public async Task<IServiceResult<List<UserRoleDto>>> GetActiveRolesAsync()
    {
        try
        {
            var items = await _unitOfWork.UserRoleRepository.GetActiveRolesAsync();
            return ServiceResult<List<UserRoleDto>>.Success(items.Select(x => x.ToDto()).Where(x => x != null).ToList());
        }
        catch (Exception ex)
        {
            return ServiceResult<List<UserRoleDto>>.Failure($"Error retrieving active user roles: {ex.Message}");
        }
    }
}