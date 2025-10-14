using EV_SCMMS.Core.Application.DTOs.Auth;
using EV_SCMMS.Core.Application.DTOs.User;
using EV_SCMMS.Core.Domain.Models;

namespace EV_SCMMS.Infrastructure.Mappings;

/// <summary>
/// Extension methods for User entity mapping
/// </summary>
public static class UserMapper
{
    /// <summary>
    /// Convert User entity to UserDto
    /// </summary>
    /// <param name="user">User entity</param>
    /// <returns>UserDto</returns>
    public static UserDto ToDto(this User user)
    {
        return new UserDto
        {
            UserId = user.Userid,
            Email = user.Email,
            FirstName = user.Firstname,
            LastName = user.Lastname,
            PhoneNumber = user.Phonenumber,
            Address = user.Address,
            Birthday = user.Birthday,
            RoleId = user.Roleid,
            RoleName = user.Role?.Name ?? string.Empty,
            Status = user.Status,
            IsActive = user.Isactive,
            CreatedAt = user.Createdat,
            UpdatedAt = user.Updatedat
        };
    }

    /// <summary>
    /// Convert list of User entities to list of UserDto
    /// </summary>
    /// <param name="users">List of User entities</param>
    /// <returns>List of UserDto</returns>
    public static List<UserDto> ToDto(this IEnumerable<User> users)
    {
        return users.Select(user => user.ToDto()).ToList();
    }

    /// <summary>
    /// Convert RegisterDto to User entity
    /// </summary>
    /// <param name="registerDto">RegisterDto</param>
    /// <returns>User entity</returns>
    public static User ToEntity(this RegisterDto registerDto)
    {
        return new User
        {
            Userid = Guid.NewGuid(),
            Email = registerDto.Email.ToLower(),
            Firstname = registerDto.FirstName,
            Lastname = registerDto.LastName,
            Phonenumber = registerDto.PhoneNumber,
            Address = registerDto.Address,
            Birthday = registerDto.Birthday,
            Status = "ACTIVE",
            Isactive = true,
            Createdat = DateTime.UtcNow,
            Updatedat = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Update User entity from RegisterDto
    /// </summary>
    /// <param name="user">User entity to update</param>
    /// <param name="registerDto">RegisterDto with new data</param>
    public static void UpdateFromDto(this User user, RegisterDto registerDto)
    {
        user.Email = registerDto.Email.ToLower();
        user.Firstname = registerDto.FirstName;
        user.Lastname = registerDto.LastName;
        user.Phonenumber = registerDto.PhoneNumber;
        user.Address = registerDto.Address;
        user.Birthday = registerDto.Birthday;
        user.Updatedat = DateTime.UtcNow;
    }
}