using AutoMapper;
using EV_SCMMS.Core.Application.DTOs;
using EV_SCMMS.Core.Application.Interfaces;
using EV_SCMMS.Core.Application.Interfaces.Services;
using EV_SCMMS.Core.Application.Results;
using EV_SCMMS.Core.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace EV_SCMMS.Infrastructure.Services;

/// <summary>
/// User service implementation with business logic
/// </summary>
public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<UserService> _logger;

    public UserService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<UserService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ServiceResult<UserDto>> GetUserByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(id, cancellationToken);
            
            if (user == null)
            {
                return ServiceResult<UserDto>.Failure("User not found");
            }

            var userDto = _mapper.Map<UserDto>(user);
            return ServiceResult<UserDto>.Success(userDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by id {UserId}", id);
            return ServiceResult<UserDto>.Failure("An error occurred while retrieving user");
        }
    }

    public async Task<ServiceResult<IEnumerable<UserDto>>> GetAllUsersAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var users = await _unitOfWork.UserRepository.GetAllAsync(cancellationToken);
            var userDtos = _mapper.Map<IEnumerable<UserDto>>(users ?? Enumerable.Empty<User>());
            
            return ServiceResult<IEnumerable<UserDto>>.Success(userDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all users");
            return ServiceResult<IEnumerable<UserDto>>.Failure("An error occurred while retrieving users");
        }
    }

    public async Task<ServiceResult<UserDto>> CreateUserAsync(UserDto userDto, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = _mapper.Map<User>(userDto);
            user.CreatedAt = DateTime.UtcNow;

            var createdUser = await _unitOfWork.UserRepository.AddAsync(user, cancellationToken);

            var createdUserDto = _mapper.Map<UserDto>(createdUser);
            return ServiceResult<UserDto>.Success(createdUserDto, "User created successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user");
            return ServiceResult<UserDto>.Failure("An error occurred while creating user");
        }
    }

    public async Task<ServiceResult<UserDto>> UpdateUserAsync(int id, UserDto userDto, CancellationToken cancellationToken = default)
    {
        try
        {
            var existingUser = await _unitOfWork.UserRepository.GetByIdAsync(id, cancellationToken);
            
            if (existingUser == null)
            {
                return ServiceResult<UserDto>.Failure("User not found");
            }

            _mapper.Map(userDto, existingUser);
            existingUser.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.UserRepository.UpdateAsync(existingUser, cancellationToken);

            var updatedUserDto = _mapper.Map<UserDto>(existingUser);
            return ServiceResult<UserDto>.Success(updatedUserDto, "User updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user {UserId}", id);
            return ServiceResult<UserDto>.Failure("An error occurred while updating user");
        }
    }

    public async Task<ServiceResult> DeleteUserAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(id, cancellationToken);
            
            if (user == null)
            {
                return ServiceResult.Failure("User not found");
            }

            await _unitOfWork.UserRepository.DeleteAsync(user, cancellationToken);

            return ServiceResult.Success("User deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user {UserId}", id);
            return ServiceResult.Failure("An error occurred while deleting user");
        }
    }
}
