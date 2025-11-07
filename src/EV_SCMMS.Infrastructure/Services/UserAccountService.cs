using EV_SCMMS.Core.Application.DTOs.User;
using EV_SCMMS.Core.Application.Interfaces;
using EV_SCMMS.Core.Application.Interfaces.Services;
using EV_SCMMS.Core.Application.Results;
using EV_SCMMS.Infrastructure.Mappings;
using EV_SCMMS.Core.Application.DTOs.UserCertificate;

namespace EV_SCMMS.Infrastructure.Services;

/// <summary>
/// Service implementation for user account management
/// </summary>
public class UserAccountService : IUserAccountService
{
    private readonly IUnitOfWork _unitOfWork;

    public UserAccountService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IServiceResult<UserAccountDto>> CreateAsync(CreateUserAccountDto dto)
    {
        try
        {
            // Check if email already exists
            var existingUser = await _unitOfWork.UserAccountRepository.GetByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                return ServiceResult<UserAccountDto>.Failure("Email already exists");
            }

            var entity = dto.ToUserAccountEntity();
            if (entity == null)
            {
                return ServiceResult<UserAccountDto>.Failure("Invalid user data");
            }
            
            // Note: In production, password should be hashed here
            // entity.Password = _passwordHashService.HashPassword(dto.Password);
            
            await _unitOfWork.UserAccountRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            var result = entity.ToUserAccountDto();
            return ServiceResult<UserAccountDto>.Success(result!, "User account created successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<UserAccountDto>.Failure($"Error creating user account: {ex.Message}");
        }
    }

    public async Task<IServiceResult<UserAccountDto>> UpdateAsync(Guid id, UpdateUserAccountDto dto)
    {
        try
        {
            var existing = await _unitOfWork.UserAccountRepository.GetByIdAsync(id);
            if (existing == null)
            {
                return ServiceResult<UserAccountDto>.Failure("User account not found");
            }

            // Check email uniqueness if email is being updated
            if (!string.IsNullOrEmpty(dto.Email) && dto.Email != existing.Email)
            {
                var emailExists = await _unitOfWork.UserAccountRepository.GetByEmailAsync(dto.Email);
                if (emailExists != null)
                {
                    return ServiceResult<UserAccountDto>.Failure("Email already exists");
                }
            }

            existing.UpdateFromUserAccountDto(dto);
            await _unitOfWork.UserAccountRepository.UpdateAsync(existing);
            await _unitOfWork.SaveChangesAsync();

            var result = existing.ToUserAccountDto();
            return ServiceResult<UserAccountDto>.Success(result!, "User account updated successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<UserAccountDto>.Failure($"Error updating user account: {ex.Message}");
        }
    }

    public async Task<IServiceResult<UserAccountDto>> UpdateRoleAsync(Guid id, Guid roleId)
    {
        try
        {
            // Check if user exists first
            var existingUser = await _unitOfWork.UserAccountRepository.GetByIdAsync(id);
            if (existingUser == null)
            {
                return ServiceResult<UserAccountDto>.Failure("User account not found");
            }

            var success = await _unitOfWork.UserAccountRepository.UpdateRoleAsync(id, roleId);
            if (!success)
            {
                return ServiceResult<UserAccountDto>.Failure("Role update failed");
            }

            var updated = await _unitOfWork.UserAccountRepository.GetByIdAsync(id);
            var result = updated?.ToUserAccountDto();
            return ServiceResult<UserAccountDto>.Success(result!, "User role updated successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<UserAccountDto>.Failure($"Error updating user role: {ex.Message}");
        }
    }

    public async Task<IServiceResult<bool>> DeleteAsync(Guid id)
    {
        try
        {
            var existing = await _unitOfWork.UserAccountRepository.GetByIdAsync(id);
            if (existing == null)
            {
                return ServiceResult<bool>.Failure("User account not found");
            }

            existing.Isactive = false;
            existing.Updatedat = DateTime.UtcNow;
            await _unitOfWork.UserAccountRepository.UpdateAsync(existing);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<bool>.Success(true, "User account deleted successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<bool>.Failure($"Error deleting user account: {ex.Message}");
        }
    }

    public async Task<IServiceResult<UserAccountDto>> GetByIdAsync(Guid id)
    {
        try
        {
            var entity = await _unitOfWork.UserAccountRepository.GetByIdAsync(id);
            if (entity == null)
            {
                return ServiceResult<UserAccountDto>.Failure("User account not found");
            }

            var result = entity.ToUserAccountDto();
            return ServiceResult<UserAccountDto>.Success(result!);
        }
        catch (Exception ex)
        {
            return ServiceResult<UserAccountDto>.Failure($"Error retrieving user account: {ex.Message}");
        }
    }

    public async Task<IServiceResult<List<UserAccountDto>>> GetAllAsync()
    {
        try
        {
            var items = await _unitOfWork.UserAccountRepository.GetAllAsync();
            return ServiceResult<List<UserAccountDto>>.Success(items.Select(x => x.ToUserAccountDto()).Where(x => x != null).ToList()!);
        }
        catch (Exception ex)
        {
            return ServiceResult<List<UserAccountDto>>.Failure($"Error retrieving user accounts: {ex.Message}");
        }
    }

    public async Task<IServiceResult<List<UserAccountDto>>> GetByRoleAsync(string role)
    {
        try
        {
            var items = await _unitOfWork.UserAccountRepository.GetByRoleAsync(role);
            return ServiceResult<List<UserAccountDto>>.Success(items.Select(x => x.ToUserAccountDto()).Where(x => x != null).ToList()!);
        }
        catch (Exception ex)
        {
            return ServiceResult<List<UserAccountDto>>.Failure($"Error retrieving users by role: {ex.Message}");
        }
    }

    public async Task<IServiceResult<List<TechnicianWithCertificatesDto>>> GetAllTechniciansAsync()
    {
        try
        {
            var technicians = await _unitOfWork.UserAccountRepository.GetByRoleAsync("TECHNICIAN");
            var result = new List<TechnicianWithCertificatesDto>();

            foreach (var tech in technicians)
            {
                var certificates = await _unitOfWork.UserCertificateRepository.GetByUserIdAsync(tech.Userid);
                var validCerts = certificates.Where(c => c.Isactive == true).Count();

                result.Add(new TechnicianWithCertificatesDto
                {
                    UserId = tech.Userid,
                    UserName = $"{tech.Firstname} {tech.Lastname}".Trim(),
                    Email = tech.Email ?? string.Empty,
                    PhoneNumber = tech.Phonenumber,
                    IsActive = tech.Isactive ?? false,
                    Certificates = certificates.Select(c => c.ToDto()).ToList(),
                    ValidCertificatesCount = validCerts,
                    ExpiredCertificatesCount = 0
                });
            }

            return ServiceResult<List<TechnicianWithCertificatesDto>>.Success(result);
        }
        catch (Exception ex)
        {
            return ServiceResult<List<TechnicianWithCertificatesDto>>.Failure($"Error retrieving technicians: {ex.Message}");
        }
    }
}