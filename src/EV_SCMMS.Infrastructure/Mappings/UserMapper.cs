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
    public static UserDto ToDto(this Useraccount user)
    {
        return new UserDto
        {
            UserId = user.Userid,
            Email = user.Email,
            FirstName = user.Firstname,
            LastName = user.Lastname,
            PhoneNumber = user.Phonenumber,
            Address = user.Address,
            RoleId = user.Roleid,
            RoleName = user.Role?.Name ?? string.Empty
        };
    }

    /// <summary>
    /// Convert list of User entities to list of UserDto
    /// </summary>
    /// <param name="users">List of User entities</param>
    /// <returns>List of UserDto</returns>
    public static List<UserDto> ToDto(this IEnumerable<Useraccount> users)
    {
        return users.Select(user => user.ToDto()).ToList();
    }

    /// <summary>
    /// Convert RegisterDto to User entity
    /// </summary>
    /// <param name="registerDto">RegisterDto</param>
    /// <returns>User entity</returns>
    public static Useraccount ToEntity(this RegisterDto registerDto)
    {
        return new Useraccount
        {
            Email = registerDto.Email.ToLower(),
            Firstname = registerDto.FirstName,
            Lastname = registerDto.LastName,
            Phonenumber = registerDto.PhoneNumber,
            Address = registerDto.Address
        };
    }

    /// <summary>
    /// Update User entity from RegisterDto
    /// </summary>
    /// <param name="user">User entity to update</param>
    /// <param name="registerDto">RegisterDto with new data</param>
    public static void UpdateFromDto(this Useraccount user, RegisterDto registerDto)
    {
        user.Email = registerDto.Email.ToLower();
        user.Firstname = registerDto.FirstName;
        user.Lastname = registerDto.LastName;
        user.Phonenumber = registerDto.PhoneNumber;
        user.Address = registerDto.Address;
    }

  /// <summary>
  /// Update User entity from RegisterDto
  /// </summary>
  /// <param name="user">User entity to update</param>
  /// <param name="registerDto">RegisterDto with new data</param>
  public static Useraccount CreateStaffToEntity(this CreateStaffDto registerDto)
  {
    return new Useraccount
    {
      Email = registerDto.Email.ToLower(),
      Firstname = registerDto.FirstName,
      Lastname = registerDto.LastName,
      Phonenumber = registerDto.PhoneNumber,
      Address = registerDto.Address
    };
  }

  // UserAccount specific mappings
  public static UserAccountDto? ToUserAccountDto(this Useraccount? entity)
  {
    if (entity == null) return null;

    return new UserAccountDto
    {
      UserId = entity.Userid,
      Email = entity.Email ?? string.Empty,
      FirstName = entity.Firstname,
      LastName = entity.Lastname,
      PhoneNumber = entity.Phonenumber,
      Address = entity.Address,
      RoleId = entity.Roleid,
      Status = entity.Status,
      IsActive = entity.Isactive ?? false,
      CreatedAt = entity.Createdat,
      UpdatedAt = entity.Updatedat
    };
  }

  public static Useraccount? ToUserAccountEntity(this CreateUserAccountDto? createDto)
  {
    if (createDto == null) return null;

    return new Useraccount
    {
      Userid = Guid.NewGuid(),
      Email = createDto.Email,
      Password = createDto.Password, // Note: Should be hashed in real implementation
      Firstname = createDto.FirstName,
      Lastname = createDto.LastName,
      Phonenumber = createDto.PhoneNumber,
      Address = createDto.Address,
      Roleid = createDto.RoleId,
      Isactive = true,
      Createdat = DateTime.UtcNow
    };
  }

  public static void UpdateFromUserAccountDto(this Useraccount entity, UpdateUserAccountDto updateDto)
  {
    if (entity == null || updateDto == null) return;

    if (!string.IsNullOrEmpty(updateDto.Email))
      entity.Email = updateDto.Email;
    
    entity.Firstname = updateDto.FirstName;
    entity.Lastname = updateDto.LastName;
    entity.Phonenumber = updateDto.PhoneNumber;
    entity.Address = updateDto.Address;
    entity.Status = updateDto.Status;
    entity.Isactive = updateDto.IsActive;
    entity.Updatedat = DateTime.UtcNow;
  }
}